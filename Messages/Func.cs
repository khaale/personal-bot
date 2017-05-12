using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using PersonalBot.Shared.Domain.Torrents.Responders;

namespace PersonalBot.Functions.Messages
{

    public class Func
    {
        public static async Task<object> Run(HttpRequestMessage req)
        {
            // Initialize the azure bot
            using (BotService.Initialize())
            {
                // Deserialize the incoming activity
                string jsonContent = await req.Content.ReadAsStringAsync();
                var activity = JsonConvert.DeserializeObject<Activity>(jsonContent);

                // authenticate incoming request and add activity.ServiceUrl to MicrosoftAppCredentials.TrustedHostNames
                // if request is authenticated
                if (!await BotService.Authenticator.TryAuthenticateAsync(req, new[] { activity }, CancellationToken.None))
                {
                    return BotAuthenticator.GenerateUnauthorizedResponse(req);
                }
                var client = new ConnectorClient(new Uri(activity.ServiceUrl));

                if (activity != null)
                {
                    // one of these will have an interface and process it
                    switch (activity.GetActivityType())
                    {
                        case ActivityTypes.Message:                            
                            await ReplyOnMessageAsync(client, activity);
                            break;
                        case ActivityTypes.ConversationUpdate:
                            await ReplyOnConversationUpdateAsync(client, activity);
                            break;
                        case ActivityTypes.ContactRelationUpdate:
                        case ActivityTypes.Typing:
                        case ActivityTypes.DeleteUserData:
                        case ActivityTypes.Ping:
                        default:
                            break;
                    }
                }
                return req.CreateResponse(HttpStatusCode.Accepted);
            }
        }

        private static async Task ReplyOnMessageAsync(ConnectorClient client, Activity request)
        {
            try
            {
                var reply = request.CreateReply();
                switch (request.Text)
                {
                    case string txt when TorrentSubscribeResponder.Match(txt):
                        await TorrentSubscribeResponder.ProcessAsync(client, request, reply);
                        break;
                    case string txt when new MarkAsSeenResponder().Match(txt):
                        await new MarkAsSeenResponder().ProcessAsync(client, reply, txt);
                        break;
                    case string txt when new MarkAsDownloadedResponder().Match(txt):
                        await new MarkAsDownloadedResponder().ProcessAsync(client, reply, txt);
                        break;
                    case string txt when new ClearTorrentStateResponder().Match(txt):
                        await new ClearTorrentStateResponder().ProcessAsync(client, reply, txt);
                        break;
                    default:
                        await TorrentListResponder.ProcessAsync(client, reply);
                        break;
                }
            }
            catch (Exception ex)
            {
                var reply = request.CreateReply();
                reply.Text = ex.ToString();
                reply.TextFormat = "plain";
                await client.Conversations.ReplyToActivityAsync(reply);
            }
        }
        
        public static async Task ReplyOnConversationUpdateAsync(ConnectorClient client, Activity activity)
        {
            IConversationUpdateActivity update = activity;
            if (update.MembersAdded.Any())
            {
                var reply = activity.CreateReply();
                var newMembers = update.MembersAdded?.Where(t => t.Id != activity.Recipient.Id);
                foreach (var newMember in newMembers)
                {
                    reply.Text = "Welcome";
                    if (!string.IsNullOrEmpty(newMember.Name))
                    {
                        reply.Text += $" {newMember.Name}";
                    }
                    reply.Text += "!";
                    await client.Conversations.ReplyToActivityAsync(reply);
                }
            }
        }
    }
}