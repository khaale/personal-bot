﻿using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;
using Messages.Model.Torrents;
using Messages.Services.Torrents;
using Microsoft.WindowsAzure.Storage.Table;

namespace Messages.Responders
{
    public class MarkAsSeenResponder : TorrentMarkResponder
    {
        protected override string CommandText => MessagesConsts.MarkAsSeenAction;

        protected override ITableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsSeen", new EntityProperty(true));

            return entity;
        }
    }

    public class MarkAsDownloadedResponder : TorrentMarkResponder
    {
        protected override string CommandText => MessagesConsts.MarkAsDownloadedAction;

        protected override ITableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsDownloaded", new EntityProperty(true));

            return entity;
        }
    }

    public class ClearTorrentStateResponder : TorrentMarkResponder
    {
        protected override string CommandText => MessagesConsts.ClearState;

        protected override ITableEntity UpdateTorrent(TorrentEntity torrentEntity)
        {
            var entity = new DynamicTableEntity(torrentEntity.PartitionKey, torrentEntity.RowKey);

            entity.ETag = "*";
            entity.Properties.Add("IsSeen", new EntityProperty(false));
            entity.Properties.Add("IsDownloaded", new EntityProperty(false));

            return entity;
        }
    }

    public abstract class TorrentMarkResponder
    {
        protected abstract string CommandText
        {
            get;
        }

        public bool Match(string text)
        {
            return text != null && text.StartsWith(CommandText);
        }

        public async Task ProcessAsync(ConnectorClient client, Activity reply, string message)
        {
            var keyString = message.Remove(0, CommandText.Length).Trim();
            var key = new TorrentKey(keyString);
            var entity = new TorrentEntity(key.TopicId, key.InfoHash);

            var toUpdate = UpdateTorrent(entity);

            var torrentRepository = new TorrentRepository();

            await torrentRepository.SaveTorrentAsync(toUpdate);
            reply.Text = $"{CommandText} - Success!";
            await client.Conversations.ReplyToActivityAsync(reply);
        }

        protected abstract ITableEntity UpdateTorrent(TorrentEntity torrentEntity);
    }
}
