using Microsoft.WindowsAzure.Storage.Blob;

namespace LightBlue.Hosted
{
    public class HostedAzureCopyState : IAzureCopyState
    {
        private readonly CopyState _copyState;

        public HostedAzureCopyState(CopyState copyState)
        {
            _copyState = copyState;
        }

        public CopyStatus Status
        {
            get { return _copyState.Status; }
        }
    }
}