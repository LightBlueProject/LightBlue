using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using LightBlue.Setup;

namespace LightBlue.Standalone
{
    public class StandaloneAzureLocalResourceSource : IAzureLocalResourceSource
    {
        private readonly IDictionary<string, StandaloneAzureLocalResource> _localResources;

        public StandaloneAzureLocalResourceSource(StandaloneConfiguration standaloneConfiguration, string dataDirectory)
        {
            var xDocument = XDocument.Load(standaloneConfiguration.ServiceDefinitionPath);

            XNamespace serviceDefinitionNamespace = xDocument.Root
                .Attributes()
                .Where(a => a.IsNamespaceDeclaration)
                .First(a => a.Value.Contains("ServiceDefinition"))
                .Value;

            var roleElement = xDocument.Root.Elements()
                .First(e => e.Attribute("name").Value == standaloneConfiguration.RoleName);

            var localResourcesElement = roleElement.Descendants(serviceDefinitionNamespace + "LocalResources")
                .FirstOrDefault();

            if (localResourcesElement == null)
            {
                _localResources = new Dictionary<string, StandaloneAzureLocalResource>();
                return;
            }

            var processId = standaloneConfiguration.RoleName
                + "-"
                + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);

            _localResources = localResourcesElement.Descendants()
                .ToDictionary(
                    e => e.Attribute("name").Value,
                    e => new StandaloneAzureLocalResource
                    {
                        Name = e.Attribute("name").Value,
                        MaximumSizeInMegabytes = Int32.Parse(e.Attribute("sizeInMB").Value),
                        RootPath = Path.Combine(
                            dataDirectory,
                            ".resources",
                            processId,
                            e.Attribute("name").Value)
                    });
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