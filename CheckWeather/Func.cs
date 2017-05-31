using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using PersonalBot.Shared.Core.Responders;
using PersonalBot.Shared.Domain.Conversations.Models;
using PersonalBot.Shared.Domain.Conversations.Services;
using PersonalBot.Shared.Domain.Weather;

namespace PersonalBot.Functions.CheckWeather
{
    public static class Func
    {
        public static async Task Run(TimerInfo timerInfo, TraceWriter log)
        {
            log.Info("Starting weather check..");

            var channel = CloudConfigurationManager.GetSetting("ReplyChannel");

            if (string.IsNullOrWhiteSpace(channel))
            {
                log.Info("Channel is empty - skipping!");
                return;
            }

            var conversations = await GetConversations(channel);
            
            var conversation = conversations.OrderByDescending(x => x.Timestamp).FirstOrDefault();

            if (conversation != null)
            {
                await SendWeather(conversation);
                log.Info("Sent!");
            }
            else
            {
                log.Info("No active conversations");
            }
        }

        private static async Task SendWeather(ConversationEntity conversation)
        {
            var reference = JsonConvert.DeserializeObject<ConversationReference>(conversation.Reference);
            var activity = reference.GetPostToUserMessage();

            var client = new ConnectorClient(new Uri(activity.ServiceUrl));
            var sender = new ProactiveMessageSender(client);

            await new WeatherResponder(sender).ProcessAsync(activity);
        }

        private static async Task<IReadOnlyCollection<ConversationEntity>> GetConversations(string channel)
        {
            var conversationRepo = new ConversationRepository();
            return await conversationRepo.GetActiveConversationsAsync(channel);
        }
    }
}
