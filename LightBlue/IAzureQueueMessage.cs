namespace LightBlue
{
    public interface IAzureQueueMessage
    {
        byte[] AsBytes { get; }
        string AsString { get; }
        int DequeueCount { get; }
    }
}