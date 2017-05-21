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
                await context.Forward(new TorrentDialog(), ResumeAfterDialog, message, CancellationToken.None);
            }
            else if (message.Text.StartsWith(Actions.Help.FullCommand))
            {
                await context.Forward(new HelpDialog(), ResumeAfterDialog, message, CancellationToken.None);
            }
            else
            {
                await context.PostAsync($"Sorry, don't understand you. Try `{Actions.Help.FullCommand}` to list available commands.");
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterDialog(IDialogContext context, IAwaitable<bool> result)
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
