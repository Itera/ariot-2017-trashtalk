using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using TrashTalkDashboard.Models;

namespace TrashTalkDashboard.Data.Repositories
{
    public interface IDocumentDbRepository
    {
        Task<Document> CreateItemAsync(Guid deviceId);
        Task<IEnumerable<TrashCanStatus>> GetItemsAsync(Expression<Func<TrashCanStatus, bool>> predicate);
        Task DeleteItemAsync(string deviceId);
    }
}