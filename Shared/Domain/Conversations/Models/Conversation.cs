using Microsoft.WindowsAzure.Storage.Table;

namespace PersonalBot.Shared.Domain.Conversations.Models
{
    public class ConversationEntity : TableEntity
    {
        public ConversationEntity()
        {
        }

        public ConversationEntity(string channel, string userId)
        {
            PartitionKey = channel;
            RowKey = userId;
        }

        public string Reference { get; set; }
        public bool IsActive { get; set; }
    }
}
