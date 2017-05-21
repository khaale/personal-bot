using System.Threading.Tasks;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Conversations.Models;
using PersonalBot.Shared.Domain.Conversations.Services;

namespace PersonalBot.Shared.Domain.Torrents.Responders
{
    public class TorrentSubscribeResponder
    {
        private readonly IMessageSender _sender;

        public TorrentSubscribeResponder(IMessageSender sender)
        {
            _sender = sender;
        }

        public async Task ProcessAsync(IMessageActivity request, IMessageActivity reply)
        {
            var entity = new ConversationEntity(request.ChannelId, request.Recipient.Id);
            entity.IsActive = true;

            var reference = request.ToConversationReference();
            entity.Reference = JsonConvert.SerializeObject(reference);

            var repo = new ConversationRepository();

            await repo.SaveAsync(entity);

            reply.Text = "You have been subscribed to new torrent announcements";
            await _sender.SendAsync(reply);
        }
    }
}
