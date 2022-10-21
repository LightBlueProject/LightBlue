using Azure.Storage.Blobs.Models;

namespace LightBlue
{
    public interface IAzureCopyState
    {
        CopyStatus Status { get; }
        string StatusDescription { get; }
    }
}