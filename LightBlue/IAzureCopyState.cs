using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue
{
    public interface IAzureCopyState
    {
        CopyStatus Status { get; }
    }
}