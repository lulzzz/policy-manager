using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using PolicyManager.DataAccess.Attributes;
using PolicyManager.DataAccess.Extensions;
using PolicyManager.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace PolicyManager.DataAccess.Repositories
{
    public interface IDataRepository<TKey, TModel>
    {
        Task InitializeDatabaseAsync(string partitionKeyPath);

        Task<TModel> FetchItemAsync(TKey partitionKey, string id);

        Task<IEnumerable<TModel>> FetchItemsAsync(TKey partitionKey);

        Task<IEnumerable<TModel>> FindItemsAsync(TKey partitionKey, Expression<Func<TModel, bool>> predicate);

        Task<TModel> CreateItemAsync(TKey partitionKey, TModel item);

        Task<TModel> CreateItemIfNotExistsAsync(TKey partitionKey, TModel item);

        Task<TModel> UpdateItemAsync(TKey partitionKey, string id, TModel item);

        Task DeleteItemAsync(TKey partitionKey, string id);
    }

    public class DataRepository<TKey, TModel>
        : IDataRepository<TKey, TModel>
        where TModel : class
    {
        readonly IDocumentClient documentClient;
        public string DatabaseId { get; private set; }
        public string CollectionId { get; private set; }

        public DataRepository(DocumentSettings documentSettings)
        {
            var connectionPolicy = new ConnectionPolicy()
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            };

            DatabaseId = documentSettings.DatabaseId;
            CollectionId = typeof(TModel).GetAttributeValue<DocumentNameAttribute, string>(a => a.Name);

            documentClient = new DocumentClient(documentSettings.DocumentEndpoint, documentSettings.DocumentKey, connectionPolicy);
        }

        public async Task InitializeDatabaseAsync(string partitionKeyPath)
        {
            await CreateDatabaseIfNotExistsAsync(DatabaseId);
            await CreateCollectionIfNotExistsAsync(partitionKeyPath, DatabaseId, CollectionId);
        }

        public async Task<TModel> FetchItemAsync(TKey partitionKey, string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            var resourceResponse = await documentClient.ReadDocumentAsync(documentUri, BuildRequestOptions(partitionKey));
            return JsonConvert.DeserializeObject<TModel>(resourceResponse.Resource.ToString());
        }

        public async Task<IEnumerable<TModel>> FetchItemsAsync(TKey partitionKey)
        {
            var documentQuery = documentClient.CreateDocumentQuery<TModel>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), BuildFeedOptions(partitionKey, -1))
                .AsDocumentQuery();

            var results = new List<TModel>();
            while (documentQuery.HasMoreResults)
            {
                results.AddRange(await documentQuery.ExecuteNextAsync<TModel>());
            }

            return results;
        }

        public async Task<IEnumerable<TModel>> FindItemsAsync(TKey partitionKey, Expression<Func<TModel, bool>> predicate)
        {
            var documentQuery = documentClient.CreateDocumentQuery<TModel>(UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId), BuildFeedOptions(partitionKey, -1))
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<TModel>();
            while (documentQuery.HasMoreResults)
            {
                results.AddRange(await documentQuery.ExecuteNextAsync<TModel>());
            }

            return results;
        }

        public async Task<TModel> CreateItemAsync(TKey partitionKey, TModel item)
        {
            var documentUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var resourceResponse = await documentClient.CreateDocumentAsync(documentUri, item, BuildRequestOptions(partitionKey));
            return JsonConvert.DeserializeObject<TModel>(resourceResponse.Resource.ToString());
        }

        public async Task<TModel> CreateItemIfNotExistsAsync(TKey partitionKey, TModel item)
        {
            var documentUri = UriFactory.CreateDocumentCollectionUri(DatabaseId, CollectionId);
            var resourceResponse = await documentClient.UpsertDocumentAsync(documentUri, item, BuildRequestOptions(partitionKey));
            return JsonConvert.DeserializeObject<TModel>(resourceResponse.Resource.ToString());
        }

        public async Task<TModel> UpdateItemAsync(TKey partitionKey, string id, TModel item)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            var resourceResponse = await documentClient.ReplaceDocumentAsync(documentUri, item, BuildRequestOptions(partitionKey));
            return JsonConvert.DeserializeObject<TModel>(resourceResponse.Resource.ToString());
        }

        public async Task DeleteItemAsync(TKey partitionKey, string id)
        {
            var documentUri = UriFactory.CreateDocumentUri(DatabaseId, CollectionId, id);
            await documentClient.DeleteDocumentAsync(documentUri, BuildRequestOptions(partitionKey));
        }

        private RequestOptions BuildRequestOptions(TKey partitionKey)
        {
            var requestParitionKey = new PartitionKey(partitionKey);
            return new RequestOptions()
            {
                PartitionKey = requestParitionKey,
            };
        }

        private FeedOptions BuildFeedOptions(TKey partitionKey, int maxItemCount)
        {
            var feedPartitionKey = new PartitionKey(partitionKey);
            return new FeedOptions()
            {
                MaxItemCount = maxItemCount,
                PartitionKey = feedPartitionKey,
            };
        }

        private async Task CreateDatabaseIfNotExistsAsync(string databaseId)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync(string partitionKeyPath, string databaseId, string collectionId)
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionId));
            }
            catch (DocumentClientException documentClientException)
            {
                if (documentClientException.StatusCode == HttpStatusCode.NotFound)
                {
                    var documentCollection = new DocumentCollection { Id = collectionId };
                    documentCollection.PartitionKey.Paths.Add(partitionKeyPath);
                    await documentClient.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(databaseId), documentCollection, new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}