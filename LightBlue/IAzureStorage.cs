namespace LightBlue
{
    public interface IAzureStorage
    {
        IAzureBlobStorage CreateAzureBlobStorageClient();
    }
}