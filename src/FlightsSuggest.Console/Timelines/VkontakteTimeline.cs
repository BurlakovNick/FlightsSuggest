using System.Collections.Generic;
using System.Linq;
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

        public void Actualize()
        {
            var offset = offsetStorage.Find(flightSource);
            var latestOffset = vkontakteClient.GetPosts(vkGroupName, 0, 10)
                .OrderByDescending(x => x.Date)
                .First()
                .Date
                .Ticks;

            if (!offset.HasValue)
            {
                offsetStorage.Write(flightSource, latestOffset);
                return;
            }

            if (offset == latestOffset)
            {
                return;
            }

            var skip = 0UL;
            while (true)
            {
                var wallPosts = vkontakteClient.GetPosts(vkGroupName, skip, (ulong)batchSize)
                    .OrderByDescending(x => x.Date)
                    .ToArray();

                foreach (var wallPost in wallPosts.Where(x => x.Date.Ticks > offset))
                {
                    var flightNews = flightNewsFactory.Create(wallPost, flightSource);
                    flightNewsStorage.Write(flightNews);
                }

                if (wallPosts.Last().Date.Ticks <= offset)
                {
                    break;
                }

                skip += (ulong) wallPosts.Length;
            }

            offsetStorage.Write(flightSource, latestOffset);
        }

        public IEnumerable<FlightNews> ReadNews(long offset)
        {
            while (true)
            {
                var flights = flightNewsStorage.Select(offset, batchSize, flightSource);
                if (flights.Length == 0)
                {
                    break;
                }

                foreach (var flight in flights)
                {
                    yield return flight;
                    offset = flight.Offset;
                }
            }
        }

        public long? LatestOffset => flightNewsStorage.FindLatestOffset(flightSource);

        public string Name => "Vkontakte";
    }
}