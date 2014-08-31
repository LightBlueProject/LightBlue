namespace LightBlue.Standalone
{
    public class BlobLocationParts
    {
        private readonly string _containerPath;
        private readonly string _blobPath;

        public BlobLocationParts(string containerPath, string blobPath)
        {
            _containerPath = containerPath;
            _blobPath = blobPath;
        }

        public string ContainerPath
        {
            get { return _containerPath; }
        }

        public string BlobPath
        {
            get { return _blobPath; }
        }
    }
}