using Microsoft.WindowsAzure.Storage.Table;

namespace PersonalBot.Shared.Domain.Torrents.Models
{
    public class TorrentEntity : TableEntity
    {
        public TorrentEntity()
        {
        }

        public TorrentEntity(Topic source)
        {
            PartitionKey = source.Id;
            RowKey = source.InfoHash;
        }

        public TorrentEntity(string id, string infoHash)
        {
            PartitionKey = id;
            RowKey = infoHash;
        }

        public bool IsSeen { get; set; }
        public bool IsDownloaded { get; set; }

        public TorrentKey GetKey()
        {
            return new TorrentKey(PartitionKey, RowKey);
        }
    }
}
