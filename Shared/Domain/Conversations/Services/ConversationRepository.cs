using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using PersonalBot.Shared.Core.Services;
using PersonalBot.Shared.Domain.Conversations.Models;

namespace PersonalBot.Shared.Domain.Conversations.Services
{
    public class ConversationRepository : RepositoryBase<ConversationEntity>
    {
        public async Task<IReadOnlyCollection<ConversationEntity>> GetActiveConversationsAsync(string channel)
        {
            CloudTable table = await GetTableAsync();

            var conversations = table.CreateQuery<ConversationEntity>()
                .AsQueryable()
                .Where(e => e.PartitionKey == channel && e.IsActive)
                .ToList();

            return conversations;
        }

        protected override string TableName => "conversation";
    }
}
