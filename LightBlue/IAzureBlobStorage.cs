namespace LightBlue
{
    public interface IAzureBlobStorage
    {
        IAzureBlobContainer GetAzureBlobContainer(string containerName);
    }
}