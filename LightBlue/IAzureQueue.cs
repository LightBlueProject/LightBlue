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
        Task DeleteIsExistsAsync();

        Task AddMessageAsync(IAzureQueueMessage message);
        Task<IAzureQueueMessage> GetMessageAsync();
        Task DeleteMessageAsync(IAzureQueueMessage message);
    }
}