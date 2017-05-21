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

namespace PersonalBot.Functions.Messages.Dialogs
{
    [Serializable]
    public class HelpDialog : IDialog<bool>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            await result;

            var reply = context.MakeMessage();
            reply.Text = GetHelpText();

            await context.PostAsync(reply);

            context.Done(true);
        }

        public string GetHelpText()
        {
            var sb = new StringBuilder();
            foreach (var action in GetActions(typeof(Actions)).Where(a => !a.Hidden))
            {
                sb.AppendFormat("- `{0}` {1}{2}", action.FullCommand, action.Description, Environment.NewLine);
            }
            return sb.ToString();
        }

        private IEnumerable<IActionDescriptor> GetActions(Type type)
        {
            var fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);

            return 
                from f in fields
                where f.FieldType.GetInterfaces().Contains(typeof(IActionDescriptor))
                select (IActionDescriptor)f.GetValue(null);
        }
    }
}