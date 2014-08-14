using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosted
{
    public class HostedAzureLocalResource : IAzureLocalResource
    {
        private readonly LocalResource _localResource;

        public HostedAzureLocalResource(LocalResource localResource)
        {
            _localResource = localResource;
        }

        public int MaximumSizeInMegabytes { get { return _localResource.MaximumSizeInMegabytes; } }
        public string Name { get { return _localResource.Name; } }
        public string RootPath { get { return _localResource.RootPath; } }
    }
}