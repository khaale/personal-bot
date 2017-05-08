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
    public class MarkAsSeenResponder
    {
        public static bool Match(string requestText)
        {
            return requestText.StartsWith(MessagesConsts.MarkAsSeenAction);
        }

        public static async Task ProcessAsync(ConnectorClient client, Activity reply, string message)
        {
            var keyString = message.Remove(0, MessagesConsts.MarkAsSeenAction.Length).Trim();
            var key = new TorrentKey(keyString);
            var entity = new TorrentEntity(key.TopicId, key.InfoHash);
            entity.SeenOn = DateTime.Now;

            var torrentRepository = new TorrentRepository();

            await torrentRepository.SaveTorrentsAsync(new[] { entity });
            reply.Text = "Marked as seen successfully!";
            await client.Conversations.ReplyToActivityAsync(reply);
        }
    }
}
