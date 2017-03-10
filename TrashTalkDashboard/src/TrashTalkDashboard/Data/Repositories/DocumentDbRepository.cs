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

        public async Task DeleteItemAsync(string deviceId)
        {
            var documents = await GetItemsAsync(x => x.id == deviceId);
            foreach (var document in documents)
            {
                await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(ConfigSettings.Database, ConfigSettings.Collection, document.id));
            }
        }

        public async Task<Document> CreateItemAsync(TrashCan trashCan)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(ConfigSettings.Database, ConfigSettings.Collection), trashCan, disableAutomaticIdGeneration: true);
        }

        public async Task<TrashCan> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(ConfigSettings.Database, ConfigSettings.Collection, id));
                return (TrashCan)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public async Task<IEnumerable<TrashCan>> GetItemsAsync(Expression<Func<TrashCan, bool>> predicate)
        {
            IDocumentQuery<TrashCan> query = _client.CreateDocumentQuery<TrashCan>(
                UriFactory.CreateDocumentCollectionUri(ConfigSettings.Database, ConfigSettings.Collection),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<TrashCan>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<TrashCan>());
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