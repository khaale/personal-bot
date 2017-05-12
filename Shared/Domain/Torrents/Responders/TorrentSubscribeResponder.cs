using System.Threading.Tasks;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using PersonalBot.Shared.Domain.Conversations.Models;
using PersonalBot.Shared.Domain.Conversations.Services;

namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class TorrentSubscribeResponder
    {
        public static bool Match(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && text.StartsWith(Actions.TorrentSubscribe.Command);
        }

        public static async Task ProcessAsync(ConnectorClient client, Activity request, Activity reply)
        {
            var entity = new ConversationEntity(request.ChannelId, request.Recipient.Id);
            entity.IsActive = true;

            var reference = request.ToConversationReference();
            entity.Reference = JsonConvert.SerializeObject(reference);

            var repo = new ConversationRepository();

            await repo.SaveAsync(entity);

            reply.Text = "You have been subscribed to new torrent announcements";
            await client.Conversations.SendToConversationAsync(reply);
        }
    }
}
