using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Messages.Model.Torrents;

namespace Messages.Services.Torrents
{
    public class TorrentRepository
    {
        private readonly CloudStorageAccount _storageAccount;

        public TorrentRepository()
        {
            _storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

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

        private async Task<CloudTable> GetTableAsync()
        {
            // Create the table client.
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("torrent");

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
