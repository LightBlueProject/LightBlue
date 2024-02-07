using System;

namespace LightBlue
{
    public class LightBlueQueueMessage
    {
        public BinaryData Body;
        public string MessageId;
        public string PopReceipt;
    }
}