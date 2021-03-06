﻿using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Infrastructure;
using FlightsSuggest.Core.Infrastructure.Vkontakte;

namespace FlightsSuggest.Core.Timelines
{
    public class VkontakteTimeline : ITimeline
    {
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
            this.offsetStorage = offsetStorage;
            this.flightNewsStorage = flightNewsStorage;
            this.vkontakteClient = vkontakteClient;
            this.flightNewsFactory = flightNewsFactory;
            flightSource = $"{Name}_{vkGroupName}";
            batchSize = 100;
            VkGroupName = vkGroupName;
        }

        public async Task ActualizeAsync()
        {
            var offset = await offsetStorage.FindAsync(flightSource);
            var latestOffset = (await vkontakteClient.GetPostsAsync(VkGroupName, 0, 10))
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
                var wallPosts = (await vkontakteClient.GetPostsAsync(VkGroupName, skip, (ulong)batchSize))
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

        public Task<long?> GetLatestOffsetAsync() => offsetStorage.FindAsync(flightSource);

        public Task WriteOffsetAsync(long offset)
        {
            return offsetStorage.WriteAsync(flightSource, offset);
        }

        public string Name => "Vkontakte";

        public string VkGroupName { get; }
    }
}