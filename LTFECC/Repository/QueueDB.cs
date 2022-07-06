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
    public class QueueDB : IQueueDB
    {
        private readonly IMongoCollection<QueueContendor> _queue;

        public QueueDB(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _queue = database.GetCollection<QueueContendor>("QueueContendor");
        }
        public async Task<bool> AddQueueAsync(QueueContendor queue)
        {
            QueueContendor existingContendor = await GetByIdNotCompletedAsync(queue.IdUser, queue.Raid);

            if(existingContendor == null)
            {
                await _queue.InsertOneAsync(queue);
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> RemoveQueueAsync(QueueContendor queue)
        {
            QueueContendor existingContendor = await GetByIdCompletedAsync(queue.IdUser, queue.Raid);

            if (existingContendor != null)
            {
                await _queue.DeleteManyAsync(x => (x.IdUser == queue.IdUser && x.Raid == queue.Raid));
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> MarkAsCompletedAsync(QueueContendor queue, string raid)
        {

            await _queue.ReplaceOneAsync(x => (x.IdUser == queue.IdUser && x.Raid == raid && !x.Completed), queue);
            return true;
        }

        public async Task<List<QueueContendor>> GetQueueByRaidNotCompletedAsync(string raid)
        {
            return await _queue.Find(x => (x.Raid == raid && !x.Completed)).ToListAsync();
        }
        public async Task<List<QueueContendor>> GetQueueByRaidCompletedAsync(string raid)
        {
            return await _queue.Find(x => (x.Raid == raid && x.Completed)).ToListAsync();
        }
        public async Task<QueueContendor> GetByIdNotCompletedAsync(string id, string raid)
        {
            return await _queue.Find<QueueContendor>(x => (x.IdUser == id && x.Raid == raid && !x.Completed)).FirstOrDefaultAsync();
        }
        public async Task<QueueContendor> GetByIdCompletedAsync(string id, string raid)
        {
            return await _queue.Find<QueueContendor>(x => (x.IdUser == id && x.Raid == raid && x.Completed)).FirstOrDefaultAsync();
        }
    }
}
