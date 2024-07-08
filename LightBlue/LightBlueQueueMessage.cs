using System;
using System.Text;

namespace LightBlue
{
    public class LightBlueQueueMessage
    {
        public LightBlueQueueMessage(BinaryData body = null, string messageId = "", string popReceipt = "")
        {
            Body = body;
            MessageId = messageId;
            PopReceipt = popReceipt;
        }

        public BinaryData Body { get; internal set; }
        public string MessageId { get; internal set; }
        public string PopReceipt { get; internal set; }

        public string AsString => Encoding.UTF8.GetString(Body.ToArray());
    }
}