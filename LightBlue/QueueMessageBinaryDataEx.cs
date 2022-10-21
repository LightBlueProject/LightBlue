using System;
using System.Text;
using Azure.Storage.Queues.Models;

namespace LightBlue
{
    public static class QueueMessageBinaryDataEx
    {
        public static string FromBase64Body(this QueueMessage queueMessage) => Encoding.UTF8.GetString(Convert.FromBase64String(queueMessage.Body.ToString()));
    }
}