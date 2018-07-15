using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.ConsoleApp.Infrastructure;
using FlightsSuggest.ConsoleApp.Infrastructure.Vkontakte;

namespace FlightsSuggest.ConsoleApp.Timelines
{
    public class VkontakteTimeline : ITimeline
    {
        private readonly string vkGroupName;
        private readonly IOffsetStorage offsetStorage;
        private readonly IFlightNewsStorage flightNewsStorage;
        private readonly IVkontakteClient vkontakteClient;
        private readonly IFlightNewsFactory flightNewsFactory;
        private readonly string flightSource;
        private readonly int batchSize;

        public VkontakteTimeline(
            string vkGroupName,
            IOffsetStorage offsetStorage,
            IFlightNewsStorage flightNewsStorage,
            IVkontakteClient vkontakteClient,
            IFlightNewsFactory flightNewsFactory
        )
        {
            this.vkGroupName = vkGroupName;
            this.offsetStorage = offsetStorage;
            this.flightNewsStorage = flightNewsStorage;
            this.vkontakteClient = vkontakteClient;
            this.flightNewsFactory = flightNewsFactory;
            flightSource = $"{Name}_{vkGroupName}";
            batchSize = 100;
        }

        public async Task ActualizeAsync()
        {
            var offset = await offsetStorage.FindAsync(flightSource);
            var latestOffset = (await vkontakteClient.GetPostsAsync(vkGroupName, 0, 10))
                .OrderByDescending(x => x.Date)
                .First()
                .Date
                .Ticks;

            if (!offset.HasValue)
            {
                await offsetStorage.WriteAsync(flightSource, latestOffset);
                return;
            }

            if (offset == latestOffset)
            {
                return;
            }

            var skip = 0UL;
            while (true)
            {
                var wallPosts = (await vkontakteClient.GetPostsAsync(vkGroupName, skip, (ulong)batchSize))
                    .OrderByDescending(x => x.Date)
                    .ToArray();

                foreach (var wallPost in wallPosts.Where(x => x.Date.Ticks > offset))
                {
                    var flightNews = flightNewsFactory.Create(wallPost, flightSource);
                    await flightNewsStorage.WriteAsync(flightNews);
                }

                if (wallPosts.Last().Date.Ticks <= offset)
                {
                    break;
                }

                skip += (ulong) wallPosts.Length;
            }

            await offsetStorage.WriteAsync(flightSource, latestOffset);
        }

        public IAsyncEnumerator<FlightNews> GetNewsEnumerator(long offset)
        {
            return new BatchedAsyncEnumerator<FlightNews, long>(
                async currentOffset => await flightNewsStorage.SelectAsync(currentOffset, batchSize, flightSource),
                flight => flight.Offset,
                offset
            );
        }

        public Task<long?> GetLatestOffsetAsync() => flightNewsStorage.FindLatestOffsetAsync(flightSource);

        public string Name => "Vkontakte";
    }
}