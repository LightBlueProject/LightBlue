using Azure.Storage.Blobs.Models;

namespace LightBlue.Standalone
{
    public class StandaloneAzureCopyState : IAzureCopyState
    {
        private readonly CopyStatus _copyStatus;
        private readonly string _statusDescription;

        public StandaloneAzureCopyState(CopyStatus copyStatus, string statusDescription)
        {
            _copyStatus = copyStatus;
            _statusDescription = statusDescription;
        }

        public string StatusDescription
        {
            get { return _statusDescription; }
        }

        public CopyStatus Status
        {
            get {  return _copyStatus; }
        }
    }
}