using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PersonalBot.Shared.Domain;

namespace PersonalBot.Functions.Messages.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            IMessageActivity message = await result;

            if (message.Text.StartsWith(Actions.TorrentsPrefix))
            {
                await context.Forward(new TorrentDialog(), ResumeAfterTorrentDialog, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync("Available commands: `!t`");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterTorrentDialog(IDialogContext context, IAwaitable<bool> result)
        {
            try
            {
                await result;
            }
            catch (Exception ex)
            {
                var message = context.MakeMessage();
                message.Text = $"**Failed with exception!**{Environment.NewLine}```{Environment.NewLine}{ex}{Environment.NewLine}```";
                await context.PostAsync(message);
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }
    }
}
