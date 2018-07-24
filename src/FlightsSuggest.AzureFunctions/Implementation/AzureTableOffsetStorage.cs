using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using FlightsSuggest.Core.Infrastructure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public class AzureTableOffsetStorage : IOffsetStorage
    {
        private readonly CloudTable offsetsTable;

        public AzureTableOffsetStorage(IFlightsConfiguration flightsConfiguration)
        {
            var storageAccount = CloudStorageAccount.Parse(flightsConfiguration.AzureTableConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            offsetsTable = tableClient.GetTableReference("Offsets");
            offsetsTable.CreateIfNotExistsAsync().Wait();
        }

        public Task WriteAsync(string id, long offset)
        {
            var offsetDbo = new OffsetDbo(id, offset);
            return offsetsTable.ExecuteAsync(TableOperation.InsertOrReplace(offsetDbo));
        }

        public async Task DeleteAsync(string id)
        {
            var result = await offsetsTable.ExecuteAsync(TableOperation.Retrieve<OffsetDbo>(id, id));
            if (result.Result == null)
            {
                return;
            }

            await offsetsTable.ExecuteAsync(TableOperation.Delete((ITableEntity) result.Result));
        }

        public async Task<long?> FindAsync(string id)
        {
            var result = await offsetsTable.ExecuteAsync(TableOperation.Retrieve<OffsetDbo>(id, id));
            var dbo = (OffsetDbo)result.Result;
            return dbo?.Offset;
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
            }

            public string Id { get; set; }
            public long Offset { get; set; }
        }
    }
}