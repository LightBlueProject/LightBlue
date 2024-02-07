namespace LightBlue.Hosted
{
    public class HostedAzureCopyState : IAzureCopyState
    {
        public HostedAzureCopyState(LightBlueBlobCopyStatus? copyStatus, string copyStatusDescription)
        {
            Status = copyStatus.Value;
            StatusDescription = copyStatusDescription;
        }

        public LightBlueBlobCopyStatus Status { get; private set; }

        public string StatusDescription { get; private set; }
    }
}