using System;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage.Table;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class AzureTableOffsetStorage : IOffsetStorage
    {
        private readonly CloudTable offsetsTable;

        public AzureTableOffsetStorage(IFlightsConfiguration flightsConfiguration)
        {
            offsetsTable = flightsConfiguration.GetAzureTableAsync("Offsets").Result;
        }

        public Task WriteAsync(string id, long offset)
        {
            return offsetsTable.WriteAsync(new OffsetDbo(id, offset));
        }

        public Task DeleteAsync(string id)
        {
            return offsetsTable.DeleteAsync<OffsetDbo>(id, id);
        }

        public async Task<long?> FindAsync(string id)
        {
            var offset = await offsetsTable.FindAsync<OffsetDbo>(id, id);
            return offset?.Offset;
        }

        public class OffsetDbo : TableEntity
        {
            public OffsetDbo()
            {
            }

            public OffsetDbo(string id, long offset)
            {
                Id = id;
                Offset = offset;

                RowKey = id;
                PartitionKey = id;
                Timestamp = DateTimeOffset.UtcNow;
            }

            public string Id { get; set; }
            public long Offset { get; set; }
        }
    }
}