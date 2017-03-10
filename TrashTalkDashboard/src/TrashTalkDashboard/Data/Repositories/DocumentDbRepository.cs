using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Data.Repositories
{
    public class DocumentDbRepository : IDocumentDbRepository
    {
        private static DocumentClient _client;

        private DatabaseSettings ConfigSettings { get; set; }

        public DocumentDbRepository(IOptions<DatabaseSettings> settings)
        {
            ConfigSettings = settings.Value;
            Initialize();
        }

        public async Task<Document> CreateItemAsync(Guid deviceId)
        {
            return
                await
                    _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigSettings.Database, ConfigSettings.Collection), deviceId);
        }

        public async Task DeleteItemAsync(string deviceId)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(ConfigSettings.Database, ConfigSettings.Collection, deviceId));
        }

        public async Task<IEnumerable<TrashCanStatus>> GetItemsAsync(Expression<Func<TrashCanStatus, bool>> predicate)
        {
            IDocumentQuery<TrashCanStatus> query = _client.CreateDocumentQuery<TrashCanStatus>(
                UriFactory.CreateDocumentCollectionUri(ConfigSettings.Database, ConfigSettings.Collection),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<TrashCanStatus>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<TrashCanStatus>());
            }

            return results;
        }

        #region Init

        private void Initialize()
        {
            _client = new DocumentClient(new Uri(ConfigSettings.Endpoint),ConfigSettings.AuthKey);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(ConfigSettings.Database));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = ConfigSettings.Database });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await
                    _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(ConfigSettings.Database, ConfigSettings.Collection));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(ConfigSettings.Database),
                        new DocumentCollection { Id = ConfigSettings.Collection },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion
    }
}