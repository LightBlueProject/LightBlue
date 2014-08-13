namespace LightBlue
{
    public interface IAzureBlobDirectory : IAzureListBlobItem
    {
        IAzureBlockBlob GetBlockBlobReference(string blobName);
    }
}