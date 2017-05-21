using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace PersonalBot.Shared.Core.Responders
{
    public interface IMessageSender
    {
        Task SendAsync(IMessageActivity activity);
    }

    public class DialogMessageSender : IMessageSender
    {
        private readonly IDialogContext _context;

        public DialogMessageSender(IDialogContext context)
        {
            _context = context;
        }

        public async Task SendAsync(IMessageActivity message)
        {
            await _context.PostAsync(message);
        }
    }

    public class ProactiveMessageSender : IMessageSender
    {
        private readonly IConnectorClient _client;

        public ProactiveMessageSender(IConnectorClient client)
        {
            _client = client;
        }

        public async Task SendAsync(IMessageActivity activity)
        {
            await _client.Conversations.SendToConversationAsync((Activity)activity);
        }
    }
}
