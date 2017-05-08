using System;
using Microsoft.WindowsAzure.Storage.Table; 

namespace Messages.Model.Torrents
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

        public DateTime? SeenOn { get; set; }

        public TorrentKey GetKey()
        {
            return new TorrentKey(PartitionKey, RowKey);
        }
    }
}
