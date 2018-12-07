using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightsSuggest.Core.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace FlightsSuggest.AzureFunctions.Implementation
{
    public static class CloudTableExtentions
    {
        public static async Task<CloudTable> GetAzureTableAsync(this IFlightsConfiguration flightsConfiguration, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(flightsConfiguration.AzureTableConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public static async Task WriteAsync<T>(this CloudTable table, T entity) where T : TableEntity
        {
            var result = await table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
            if (result.HttpStatusCode != 204)
            {
                throw new InvalidOperationException($"Can't write entity {JsonConvert.SerializeObject(entity, Formatting.Indented)}");
            }
        }

        public static async Task DeleteAsync<T>(this CloudTable table, string partition, string row) where T : TableEntity
        {
            var result = await table.ExecuteAsync(TableOperation.Retrieve<T>(partition, row));
            if (result.Result == null)
            {
                return;
            }

            await table.ExecuteAsync(TableOperation.Delete((ITableEntity)result.Result));
        }

        public static async Task<T> FindAsync<T>(this CloudTable table, string partition, string row) where T : TableEntity
        {
            var retrieveResult = await table.ExecuteAsync(TableOperation.Retrieve<T>(partition, row));
            return (T) retrieveResult.Result;
        }

        public static async Task<T[]> SelectTopAsync<T>(this CloudTable table, string partition, int take) where T : TableEntity, new()
        {
            var tableQuery = new TableQuery<T>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition))
                .Take(take);

            return await Select(table, tableQuery, take);
        }

        public static async Task<T[]> SelectAsync<T>(this CloudTable table, string partition, string from, int take) where T : TableEntity, new()
        {
            var tableQuery = new TableQuery<T>()
                .Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.GreaterThan, from)
                ))
                .Take(take);

            return await Select(table, tableQuery, take);
        }

        private static async Task<T[]> Select<T>(CloudTable table, TableQuery<T> tableQuery, int take) where T : TableEntity, new()
        {
            TableContinuationToken tableContinuationToken = null;
            var result = new List<T>();
            do
            {
                var querySegment = await table.ExecuteQuerySegmentedAsync(tableQuery, tableContinuationToken);
                tableContinuationToken = querySegment.ContinuationToken;
                result.AddRange(querySegment.Results);
            } while (tableContinuationToken != null && result.Count < take);

            return result.Take(take).ToArray();
        }
    }
}