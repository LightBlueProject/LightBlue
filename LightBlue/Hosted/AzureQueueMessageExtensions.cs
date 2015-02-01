using System;

using Microsoft.WindowsAzure.Storage.Queue;

namespace LightBlue.Hosted
{
    internal static class AzureQueueMessageExtensions
    {
        public static CloudQueueMessage ToCloudQueueMessage(this IAzureQueueMessage message)
        {
            var hostedAzureQueueMessage = message as HostedAzureQueueMessage;
            if (hostedAzureQueueMessage == null)
            {
                throw new ArgumentException("You can only add a message from the same hosting environment");
            }

            return hostedAzureQueueMessage.CloudQueueMessage;
        }
    }
}