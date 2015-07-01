using System;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue
{
    public interface IAzureQueue
    {
        Uri Uri { get; }
        string Name { get; }

        Task CreateIfNotExistsAsync();
        Task DeleteAsync();
        Task DeleteIfExistsAsync();

        Task AddMessageAsync(CloudQueueMessage message);
        Task<CloudQueueMessage> GetMessageAsync();
        Task DeleteMessageAsync(CloudQueueMessage message);

        string GetSharedAccessSignature(SharedAccessQueuePolicy policy);
    }
}