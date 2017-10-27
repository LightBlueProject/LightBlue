using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LightBlue.Setup;

namespace LightBlue.Standalone
{
    public class StandaloneAzureLocalResourceSource : IAzureLocalResourceSource
    {
        private readonly IDictionary<string, StandaloneAzureLocalResource> _localResources;

        public StandaloneAzureLocalResourceSource(StandaloneConfiguration standaloneConfiguration, string dataDirectory)
        {
            _localResources = new Dictionary<string, StandaloneAzureLocalResource>();
        }

        public IAzureLocalResource this[string index]
        {
            get
            {
                if (!_localResources.ContainsKey(index))
                {
                    throw LightBlueConfiguration.RoleEnvironmentExceptionCreator(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown resource '{0}'",
                        index));
                }

                var localResource = _localResources[index];

                Directory.CreateDirectory(localResource.RootPath);

                return localResource;
            }
        }
    }
}