using LTFECC.Models;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTFECC.Repository
{
    public interface IQueueDB
    {
        Task<List<QueueContendor>> GetQueueByRaidCompletedAsync(string raid);
        Task<List<QueueContendor>> GetQueueByRaidNotCompletedAsync(string raid);
        Task<QueueContendor> GetByIdNotCompletedAsync(string id, string raid);
        Task<QueueContendor> GetByIdCompletedAsync(string id, string raid);
        Task<bool> AddQueueAsync(QueueContendor queue);
        Task<bool> RemoveQueueAsync(QueueContendor queue);
        Task<bool> MarkAsCompletedAsync(QueueContendor queue, string raid);
    }
}
