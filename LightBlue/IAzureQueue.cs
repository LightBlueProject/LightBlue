using System;
using System.Threading.Tasks;

namespace LightBlue
{
    public interface IAzureQueue
    {
        Uri Uri { get; }
        string Name { get; }

        Task CreateIfNotExistsAsync();
        Task DeleteAsync();
        Task DeleteIfExistsAsync();

        Task AddMessageAsync(string message);
        Task<LightBlueQueueMessage> GetMessageAsync();
        Task DeleteMessageAsync(string messageId, string popReceipt);

        string GetSharedAccessReadSignature(DateTimeOffset expiresOn);
    }
}