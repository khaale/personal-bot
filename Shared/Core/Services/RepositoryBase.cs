using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace PersonalBot.Shared.Core.Services
{
    public abstract class RepositoryBase<T>
        where T : class, ITableEntity, new()
    {
        protected abstract string TableName { get; }

        private readonly CloudStorageAccount _storageAccount;

        public RepositoryBase()
        {
            _storageAccount =
                CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
        }

        public async Task SaveAsync(T entity)
        {
            CloudTable table = await GetTableAsync();

            var op = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(op);
        }

        public async Task SaveAsync(DynamicTableEntity entity)
        {
            CloudTable table = await GetTableAsync();

            var op = TableOperation.InsertOrMerge(entity);
            await table.ExecuteAsync(op);
        }

        protected async Task<CloudTable> GetTableAsync()
        {
            // Create the table client.
            CloudTableClient tableClient = _storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference(TableName);

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}