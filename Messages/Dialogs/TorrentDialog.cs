using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain;
using PersonalBot.Shared.Domain.Torrents.Responders;

namespace PersonalBot.Functions.Messages.Dialogs
{
    [Serializable]
    public class TorrentDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;
            var sender = new DialogMessageSender(context);

            var reply = context.MakeMessage();
            var text = message.Text;

            bool matched = false;

            // seen
            matched = matched || await Actions.TorrentSeen.MatchAndDoAsync(text,
                          arg => new MarkAsSeenResponder(sender).ProcessAsync(reply, arg));
            // downloaded
            matched = matched || await Actions.TorrrentDownloaded.MatchAndDoAsync(text,
                          arg => new MarkAsDownloadedResponder(sender).ProcessAsync(reply, arg));
            // reset state
            matched = matched || await Actions.TorrentResetState.MatchAndDoAsync(text,
                          arg => new ClearTorrentStateResponder(sender).ProcessAsync(reply, arg));
            // subscribe
            matched = matched || await Actions.TorrentSubscribe.MatchAndDoAsync(text,
                          arg => new TorrentSubscribeResponder(sender).ProcessAsync(message, reply));
            // list
            matched = matched || await Actions.TorrentList.MatchAndDoAsync(text,
                          arg => new TorrentListResponder(sender).ProcessAsync(reply));

            context.Done(matched);
        }
    }
}