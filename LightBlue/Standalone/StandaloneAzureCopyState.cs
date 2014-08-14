using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Standalone
{
    public class StandaloneAzureCopyState : IAzureCopyState
    {
        private readonly CopyStatus _copyStatus;

        public StandaloneAzureCopyState(CopyStatus copyStatus)
        {
            _copyStatus = copyStatus;
        }

        public CopyStatus Status
        {
            get {  return _copyStatus; }
        }
    }
}