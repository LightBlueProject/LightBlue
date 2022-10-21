using Azure.Storage.Blobs.Models;

namespace LightBlue.Hosted
{
    public class HostedAzureCopyState : IAzureCopyState
    {
        public HostedAzureCopyState(CopyStatus? copyStatus, string copyStatusDescription)
        {
            Status = copyStatus.Value;
            StatusDescription = copyStatusDescription;
        }

        public CopyStatus Status { get; private set; }

        public string StatusDescription { get; private set; }
    }
}