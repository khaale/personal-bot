using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.WindowsAzure.Storage.Table;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Torrents.Models;
using PersonalBot.Shared.Domain.Torrents.Services;

namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class MarkAsSeenResponder : TorrentMarkResponder
    {
        protected override DynamicTableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsSeen", new EntityProperty(true));

            return entity;
        }

        public MarkAsSeenResponder(IMessageSender sender) : base(sender)
        {
        }
    }

    public class MarkAsDownloadedResponder : TorrentMarkResponder
    {
        protected override DynamicTableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsDownloaded", new EntityProperty(true));

            return entity;
        }

        public MarkAsDownloadedResponder(IMessageSender sender) : base(sender)
        {
        }
    }

    public class ClearTorrentStateResponder : TorrentMarkResponder
    {
        protected override DynamicTableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsSeen", new EntityProperty(false));
            entity.Properties.Add("IsDownloaded", new EntityProperty(false));

            return entity;
        }

        public ClearTorrentStateResponder(IMessageSender sender) : base(sender)
        {
        }
    }

    public abstract class TorrentMarkResponder
    {
        private readonly IMessageSender _sender;

        protected TorrentMarkResponder(IMessageSender sender)
        {
            _sender = sender;
        }

        public async Task ProcessAsync(IMessageActivity reply, TorrentKey key)
        {
            var entity = new TorrentEntity(key.TopicId, key.InfoHash);

            var toUpdate = UpdateTorrent(entity);

            var torrentRepository = new TorrentRepository();

            await torrentRepository.SaveAsync(toUpdate);
            reply.Text = "Success!";
            await _sender.SendAsync(reply);
        }

        protected abstract DynamicTableEntity UpdateTorrent(TorrentEntity torrentEntity);
    }
}
