namespace LightBlue
{
    public interface IAzureBlobStorageClient
    {
        IAzureBlobContainer GetBlockBlobReference(string containerName);
    }
}