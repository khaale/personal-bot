using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using PersonalBot.Shared.Core.Services;
using PersonalBot.Shared.Domain.Torrents.Models;

namespace PersonalBot.Shared.Domain.Torrents.Services
{
    public class TorrentRepository : RepositoryBase<TorrentEntity>
    {
        public async Task SaveTorrentsAsync(IEnumerable<TorrentEntity> torrents)
        {
            CloudTable table = await GetTableAsync();
            
            var opTasks =
                from t in torrents
                let op = TableOperation.InsertOrMerge(t)
                select table.ExecuteAsync(op);

            await Task.WhenAll(opTasks.ToArray());
        }
        
        public async Task<IReadOnlyCollection<TorrentEntity>> GetTorrentsAsync(IEnumerable<TorrentKey> keys)
        {
            CloudTable table = await GetTableAsync();

            var opTasks =
                from key in keys
                let op = TableOperation.Retrieve<TorrentEntity>(key.TopicId, key.InfoHash)
                select table.ExecuteAsync(op);

            var results = await Task.WhenAll(opTasks.ToArray());

            var torrents =
                from r in results
                where r.Result != null
                select (TorrentEntity)r.Result;

            return torrents.ToList();
        }

        protected override string TableName => "torrent";
    }
}
