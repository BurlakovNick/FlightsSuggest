using System;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Timelines;
using Microsoft.WindowsAzure.Storage.Table;

namespace FlightsSuggest.AzureFunctions.Implementation.Storage
{
    public class AzureTableFlightNewsStorage : IFlightNewsStorage
    {
        private const string MaxTicksRowKey = "max_ticks";
        private readonly CloudTable flightNewsTable;

        public AzureTableFlightNewsStorage(IFlightsConfiguration flightsConfiguration)
        {
            flightNewsTable = flightsConfiguration.GetAzureTableAsync("FlightNews").Result;
        }

        public async Task WriteAsync(FlightNews flight)
        {
            await flightNewsTable.WriteAsync(new FlightNewsDbo(flight));

            var latestOffset = await FindLatestOffsetAsync(flight.Source);
            if (!latestOffset.HasValue || latestOffset.Value < flight.Date.Ticks)
            {
                await flightNewsTable.WriteAsync(new MaxTicksDbo(flight.Source, flight.Date.Ticks));
            }
        }

        public Task DeleteAsync(FlightNews flight)
        {
            return flightNewsTable.DeleteAsync<FlightNewsDbo>(flight.Source, ToRowKey(flight.Date.Ticks));
        }

        public async Task<FlightNews[]> SelectAsync(long offset, int count, string source)
        {
            var dbos = await flightNewsTable.SelectAsync<FlightNewsDbo>(source, ToRowKey(offset), count);
            return dbos
                .Select(dbo => new FlightNews
                {
                    Id = dbo.Id,
                    Date = new DateTime(dbo.DateTicks),
                    Source = dbo.Source,
                    Url = dbo.Url,
                    Text = dbo.Text,
                    NormalizedText = dbo.Text.ToLower()
                })
                .ToArray();
        }

        public async Task<long?> FindLatestOffsetAsync(string source)
        {
            var maxTicks = await flightNewsTable.FindAsync<MaxTicksDbo>(GetMaxTicksPartitionKey(source), MaxTicksRowKey);
            return maxTicks?.DateTicks;
        }

        private static string ToRowKey(long ticks) => ticks.ToString("0:D19");

        private static string GetMaxTicksPartitionKey(string source) => MaxTicksRowKey + "_" + source;

        public class FlightNewsDbo : TableEntity
        {
            public FlightNewsDbo()
            {
            }

            public FlightNewsDbo(FlightNews flightNews)
            {
                PartitionKey = flightNews.Source;
                RowKey = ToRowKey(flightNews.Date.Ticks);
                Timestamp = DateTimeOffset.UtcNow;

                Id = flightNews.Id;
                DateTicks = flightNews.Date.Ticks;
                Url = flightNews.Url;
                Text = flightNews.Text;
                Source = flightNews.Source;
            }

            public string Id { get; set; }
            public long DateTicks { get; set; }
            public string Url { get; set; }
            public string Text { get; set; }
            public string Source { get; set; }
        }

        public class MaxTicksDbo : TableEntity
        {
            public MaxTicksDbo()
            {
            }

            public MaxTicksDbo(string source, long dateTicks)
            {
                PartitionKey = GetMaxTicksPartitionKey(source);
                RowKey = MaxTicksRowKey;
                Timestamp = DateTimeOffset.UtcNow;

                Source = source;
                DateTicks = dateTicks;
            }

            public string Source { get; set; }
            public long DateTicks { get; set; }
        }
    }
}