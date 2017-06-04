using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain;
using PersonalBot.Shared.Domain.Torrents.Responders;
using PersonalBot.Shared.Domain.Weather;

namespace PersonalBot.Functions.Messages.Dialogs
{
    [Serializable]
    public class WeatherDialog : IDialog<bool>
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

            await new WeatherResponder(sender).ProcessAsync(reply);
            context.Done(true);
        }
    }
}