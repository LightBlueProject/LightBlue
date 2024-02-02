namespace LightBlue.Standalone
{
    public class StandaloneAzureCopyState : IAzureCopyState
    {
        private readonly LightBlueBlobCopyStatus _copyStatus;
        private readonly string _statusDescription;

        public StandaloneAzureCopyState(LightBlueBlobCopyStatus copyStatus, string statusDescription)
        {
            _copyStatus = copyStatus;
            _statusDescription = statusDescription;
        }

        public string StatusDescription
        {
            get { return _statusDescription; }
        }

        public LightBlueBlobCopyStatus Status
        {
            get { return _copyStatus; }
        }
    }
}