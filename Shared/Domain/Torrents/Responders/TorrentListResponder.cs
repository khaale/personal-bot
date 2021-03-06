﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Bot.Connector;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Torrents.Models;
using PersonalBot.Shared.Domain.Torrents.Services;

namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class TorrentListResponder
    {
        private readonly IMessageSender _sender;

        public static string[] TopicIds
        {
            get
            {
                var topicIdsString = CloudConfigurationManager.GetSetting("TopicIds");

                if (topicIdsString == null)
                {
                    throw new ApplicationException("TopicIds setting must be provided");
                }

                return topicIdsString.Split(',');
            }
        }

        public TorrentListResponder(IMessageSender sender)
        {
            _sender = sender;
        }

        public async Task ProcessAsync(IMessageActivity reply, bool sendWhenNew = false)
        {
            var repository = new TorrentRepository();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = reply.Attachments ?? new List<Attachment>();

            // Get torrent topics from Rutracker
            var rutrackerService = new RutrackerService();
            var topics = await rutrackerService.GetTopicsAsync(TopicIds);

            // Get saved torrents
            var keys = topics.Select(t => new TorrentKey(t.Id, t.InfoHash));
            var existingTorrents = await repository.GetTorrentsAsync(keys);

            var data = GetTopicsAndExistingData(topics, existingTorrents).ToList();
            
            var newTorrents = data.Where(x => x.Item2 == null).Select(x => new TorrentEntity(x.Item1)).ToList();

            if (!sendWhenNew || newTorrents.Any())
            {
                // Fill and send reply message
                FillReply(reply, data);

                await _sender.SendAsync(reply);
            }

            // Save new torrents to the storage
            await repository.SaveTorrentsAsync(newTorrents);
        }

        private static IEnumerable<(Topic,TorrentEntity)> GetTopicsAndExistingData(IEnumerable<Topic> topics, IEnumerable<TorrentEntity> existingTorrents)
        {
            return
                from topic in topics
                join e in existingTorrents on topic.GetKey() equals e.GetKey() into j
                from e in j.DefaultIfEmpty()
                let entity = e
                select (topic, entity);
        }

        private static void FillReply(IMessageActivity reply, IReadOnlyCollection<(Topic, TorrentEntity)> topics)
        {
            topics
                .OrderBy(x => x.Item2?.IsSeen)
                .ThenBy(x => x.Item2?.IsDownloaded)
                .ThenByDescending(t => t.Item1.RegTime)
                .Select(t => new TorrentPresenter(t.Item1, t.Item2))
                .ToList()
                .ForEach(t =>
                {
                    FillReply(reply, t);
                });
        }

        private static void FillReply(IMessageActivity reply, TorrentPresenter t)
        {
            var card = new HeroCard
            {
                Title = t.Title,
                Subtitle = 
                    (t.IsDownloaded ? "\u25bc " : "")
                    + (t.IsSeen ? "\u2714 " : "")
                    + t.Updated,
                Text = t.Series
            };

            card.Buttons = new List<CardAction>
                    {
                        new CardAction(
                            title: "Go to topic",
                            value: t.TopicUrl,
                            type:"openUrl")
                    };

            if (t.IsSeen)
            {
                card.Buttons.Add(
                    CreateTorrentStateChangeButton(t.Key, Actions.TorrentResetState));
            }
            else if (t.IsDownloaded)
            {
                card.Buttons.Add(
                    CreateTorrentStateChangeButton(t.Key, Actions.TorrentSeen));
            }
            else
            {
                card.Buttons.Add(
                    CreateDownloadButton(t.DownloadUrl));
                card.Buttons.Add(
                    CreateTorrentStateChangeButton(t.Key, Actions.TorrrentDownloaded));
            }
            
            reply.Attachments.Add(card.ToAttachment());
        }

        private static CardAction CreateTorrentStateChangeButton(TorrentKey key, TorrentAction action)
        {
            return new CardAction(
                            title: action.Caption,
                            value: action.ToMessage(key),
                            type: "imBack");
        }

        private static CardAction CreateDownloadButton(string downloadUrl)
        {
            return new CardAction(
                            title: "Download torrent",
                            value: downloadUrl,
                            type: "openUrl");
        }
    }
}
