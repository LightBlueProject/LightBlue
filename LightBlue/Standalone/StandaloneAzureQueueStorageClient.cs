﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using LightBlue.Infrastructure;

namespace LightBlue.Standalone
{
    public class StandaloneAzureQueueStorageClient : IAzureQueueStorageClient
    {
        private readonly string _queueStorageDirectory;

        public StandaloneAzureQueueStorageClient(string storageAccountDirectory)
        {
            StringValidation.NotNullOrWhitespace(storageAccountDirectory, "storageAccountDirectory");

            _queueStorageDirectory = Path.Combine(storageAccountDirectory, "queues");

            Directory.CreateDirectory(_queueStorageDirectory);
        }

        public IAzureQueue GetQueueReference(string queueName)
        {
            return new StandaloneAzureQueue(_queueStorageDirectory, queueName);
        }

        public IEnumerable<IAzureQueue> ListQueues()
        {
            return Directory.GetDirectories(_queueStorageDirectory)
                .Select(d => new StandaloneAzureQueue(d));
        }
    }
}