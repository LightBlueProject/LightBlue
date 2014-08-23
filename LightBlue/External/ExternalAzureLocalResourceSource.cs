using System;

namespace LightBlue.External
{
    public class ExternalAzureLocalResourceSource : IAzureLocalResourceSource
    {
        public IAzureLocalResource this[string index]
        {
            get
            {
                throw new InvalidOperationException("Cannot retrieve resources when running external to a host.");
            }
        }
    }
}