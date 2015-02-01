using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Hosted
{
    public class HostedAzureQueueMessage : IAzureQueueMessage
    {
        private readonly CloudQueueMessage _cloudQueueMessage;

        public HostedAzureQueueMessage(CloudQueueMessage cloudQueueMessage)
        {
            _cloudQueueMessage = cloudQueueMessage;
        }

        public byte[] AsBytes
        {
            get { return _cloudQueueMessage.AsBytes; }
        }

        public string AsString
        {
            get { return _cloudQueueMessage.AsString; }
        }

        public int DequeueCount
        {
            get { return _cloudQueueMessage.DequeueCount; }
        }

        internal CloudQueueMessage CloudQueueMessage
        {
            get { return _cloudQueueMessage; }
        }
    }
}