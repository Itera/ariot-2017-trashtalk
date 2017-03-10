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
        Task<IEnumerable<TrashCan>> GetItemsAsync(Expression<Func<TrashCan, bool>> predicate);
        Task DeleteItemAsync(string deviceId);
    }
}