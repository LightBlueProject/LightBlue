namespace LightBlue
{
    public interface IAzureBlobStorageClient
    {
        IAzureBlobContainer GetContainerReference(string containerName);
    }
}