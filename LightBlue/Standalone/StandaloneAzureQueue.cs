using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using LightBlue.Infrastructure;

using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Standalone
{
    public class StandaloneAzureQueue : IAzureQueue
    {
        private static readonly Action<CloudQueueMessage, string> _idAssigner = CloudQueueMessageAccessorFactory.BuildIdAssigner();

        private readonly Random _random = new Random();
        private readonly string _queueDirectory;
        private readonly ConcurrentDictionary<string, FileStream> _fileLocks = new ConcurrentDictionary<string, FileStream>(); 

        public StandaloneAzureQueue(string queueStorageDirectory, string queueName)
        {
            _queueDirectory = Path.Combine(queueStorageDirectory, queueName);
        }

        public StandaloneAzureQueue(string queueDirectory)
        {
            _queueDirectory = queueDirectory;
        }

        public Uri Uri
        {
            get { return null; }
        }

        public string Name
        {
            get { return null; }
        }

        public Task CreateIfNotExistsAsync()
        {
            Directory.CreateDirectory(_queueDirectory);

            return Task.FromResult(new object());
        }

        public Task DeleteAsync()
        {
            return null;
        }

        public Task DeleteIfExistsAsync()
        {
            return null;
        }

        public async Task AddMessageAsync(CloudQueueMessage message)
        {
            var tries = 0;
            while (true)
            {
                var fileName = Path.Combine(
                    _queueDirectory,
                    DateTimeOffset.UtcNow.Ticks.ToString());

                try
                {
                    using (var file = File.Open(fileName, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                    {
                        var buffer = message.AsBytes;
                        await file.WriteAsync(buffer, 0, buffer.Length);
                        return;
                    }
                }
                catch (IOException)
                {
                    if (tries++ >= 3)
                    {
                        throw;
                    }
                }

                await Task.Delay(_random.Next(50, 1000));
            }
        }

        public async Task<CloudQueueMessage> GetMessageAsync()
        {
            var files = Directory.GetFiles(_queueDirectory).OrderBy(f => f);

            foreach (var file in files)
            {
                FileStream fileStream = null;
                try
                {
                    var messageId = Path.GetFileName(file);
                    if (messageId == null)
                    {
                        throw new InvalidOperationException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Found a file path '{0}' with a null filename",
                                file));
                    }

                    fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Delete);

                    var messageLength = (int) fileStream.Length;

                    var buffer = new byte[messageLength];
                    
                    await fileStream.ReadAsync(buffer, 0, messageLength);

                    var cloudQueueMessage = new CloudQueueMessage(buffer);

                    _idAssigner(cloudQueueMessage, messageId);

                    _fileLocks.AddOrUpdate(messageId, fileStream, (s, stream) => fileStream);

                    return cloudQueueMessage;
                }
                catch (IOException)
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                    }
                }
                catch (Exception)
                {
                    if (fileStream != null)
                    {
                        fileStream.Dispose();
                    }

                    throw;
                }
            }

            return null;
        }

        public Task DeleteMessageAsync(CloudQueueMessage message)
        {
            var filePath = Path.Combine(_queueDirectory, message.Id);
            File.Delete(filePath);
            var fileStream = _fileLocks[message.Id];
            if (fileStream != null)
            {
                fileStream.Dispose();
            }
            return Task.FromResult(new object());
        }
    }
}