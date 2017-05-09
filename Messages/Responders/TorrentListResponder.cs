using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages.Model.Torrents;
using Messages.Services.Torrents;

namespace Messages.Responders
{
    public class TorrentListResponder
    {
        public static async Task ProcessAsync(ConnectorClient client, Activity reply, string[] topicIds)
        {
            var repository = new TorrentRepository();
            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            // Get torrent topics from Rutracker
            var rutrackerService = new RutrackerService();
            var topics = await rutrackerService.GetTopicsAsync(topicIds);

            // Get saved torrents
            var keys = topics.Select(t => new TorrentKey(t.Id, t.InfoHash));
            var existingTorrents = await repository.GetTorrentsAsync(keys);

            var data = GetTopicsAndExistingData(topics, existingTorrents).ToList();

            // Fill and send reply message
            FillReply(reply, data);
            await client.Conversations.ReplyToActivityAsync(reply);

            // Save new torrents to the storage
            var newTorrents = data.Where(x => x.Item2 == null).Select(x => new TorrentEntity(x.Item1)).ToList();
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

        private static void FillReply(Activity reply, IReadOnlyCollection<(Topic, TorrentEntity)> topics)
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

        private static void FillReply(Activity reply, TorrentPresenter t)
        {
            var card = new HeroCard
            {
                Title = t.Title,
                Subtitle = 
                    (t.IsDownloaded ? "\u25bc " : "")
                    + (t.IsSeen ? "\u2714 " : "")
                    + t.Updated,
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
                    CreateTorrentStateChangeButton(t.Key, MessagesConsts.ClearState));
            }
            else if (t.IsDownloaded)
            {
                card.Buttons.Add(
                    CreateTorrentStateChangeButton(t.Key, MessagesConsts.MarkAsSeenAction));
            }
            else
            {
                card.Buttons.Add(
                    CreateDownloadButton(t.DownloadUrl));
                card.Buttons.Add(
                    CreateTorrentStateChangeButton(t.Key, MessagesConsts.MarkAsDownloadedAction));
            }


            reply.Attachments.Add(card.ToAttachment());
        }

        private static CardAction CreateTorrentStateChangeButton(TorrentKey key, string command)
        {
            return new CardAction(
                            title: command,
                            value: $"{command} {key}",
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
