using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using PolicyManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolicyManager.DataAccess.Repositories
{
    public interface IDataRepository<TModel>
    {
        Task<TModel> CreateItemAsync(TModel item);

        Task<IEnumerable<string>> FetchPartitionKeysAsync();

        Task<IEnumerable<TModel>> ReadItemsAsync(string partitionKey);

        Task<TModel> ReadItemAsync(string partitionKey, string id);

        Task<TModel> UpdateItemAsync(TModel item);

        Task DeleteItemAsync(string partitionKey, string id);

        Task InitializeDatabaseAsync();
    }

    public class StorageRepository<TModel>
        : IDataRepository<TModel>
        where TModel : class, ITableEntity, new()
    {
        private readonly CloudTable cloudTable;

        public StorageRepository(StorageSettings storageSettings)
        {
            var cloudStorageAccount = CloudStorageAccount.Parse(storageSettings.ConnectionString);
            var cloudTableClient = cloudStorageAccount.CreateCloudTableClient();
            cloudTable = cloudTableClient.GetTableReference(typeof(TModel).Name);
        }

        public async Task<IEnumerable<string>> FetchPartitionKeysAsync()
        {
            var projectionQuery = new TableQuery<DynamicTableEntity>().Select(new string[] { "PartitionKey" });
            string entityResolver(string partitionKey, string rowKey, DateTimeOffset taskScheduler, IDictionary<string, EntityProperty> props, string etag) => props.ContainsKey("PartitionKey") ? props["PartitionKey"].StringValue : string.Empty;

            var results = new List<string>();
            var tableContinuationToken = new TableContinuationToken();

            while (tableContinuationToken != null)
            {
                var tableResult = await cloudTable.ExecuteQuerySegmentedAsync(projectionQuery, entityResolver, tableContinuationToken);
                results.AddRange(tableResult.Results);
                tableContinuationToken = tableResult.ContinuationToken;
            }

            return results;
        }

        public async Task<TModel> CreateItemAsync(TModel item)
        {
            var insertOperation = TableOperation.Insert(item);
            var tableResult = await cloudTable.ExecuteAsync(insertOperation);
            return tableResult.Result as TModel;
        }

        public async Task<IEnumerable<TModel>> ReadItemsAsync(string partitionKey)
        {
            var tableQuery = new TableQuery<TModel>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            var tableContinuationToken = new TableContinuationToken();
            var results = new List<TModel>();
            while (tableContinuationToken != null)
            {
                var tableResult = await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, tableContinuationToken);
                results.AddRange(tableResult.Results);
                tableContinuationToken = tableResult.ContinuationToken;
            }

            return results;
        }

        public async Task<TModel> ReadItemAsync(string partitionKey, string id)
        {
            var retrieveOperation = TableOperation.Retrieve<TModel>(partitionKey, id);
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation);
            return tableResult.Result as TModel;
        }

        public async Task<TModel> UpdateItemAsync(TModel item)
        {
            var insertOrReplaceOperation = TableOperation.InsertOrReplace(item);
            var tableResult = await cloudTable.ExecuteAsync(insertOrReplaceOperation);
            return tableResult.Result as TModel;
        }

        public async Task DeleteItemAsync(string partitionKey, string id)
        {
            var retrieveOperation = TableOperation.Retrieve<TModel>(partitionKey, id);
            var tableResult = await cloudTable.ExecuteAsync(retrieveOperation);
            var deleteOperation = TableOperation.Delete(tableResult.Result as ITableEntity);
            await cloudTable.ExecuteAsync(deleteOperation);
        }

        public async Task InitializeDatabaseAsync()
        {
            await cloudTable.CreateIfNotExistsAsync();
        }
    }
}
