using System;
using System.Threading.Tasks;
using Azure.Storage.Queues.Models;
using Azure.Storage.Sas;

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
        Task<QueueMessage> GetMessageAsync();
        Task DeleteMessageAsync(string messageId, string popReceipt);

        string GetSharedAccessSignature(QueueSasPermissions permissions, DateTimeOffset expiresOn);
    }
}