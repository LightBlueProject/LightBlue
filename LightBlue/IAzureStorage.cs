namespace LightBlue
{
    public interface IAzureStorage
    {
        IAzureBlobStorageClient CreateAzureBlobStorageClient();
        IAzureQueueStorageClient CreateAzureQueueStorageClient();
    }
}