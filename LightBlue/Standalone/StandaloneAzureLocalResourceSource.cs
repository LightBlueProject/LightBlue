using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Standalone
{
    public class StandaloneAzureLocalResourceSource : IAzureLocalResourceSource
    {
        private readonly Func<string, RoleEnvironmentException> _exceptionFunc;
        private readonly IDictionary<string, StandaloneAzureLocalResource> _localResources;

        public StandaloneAzureLocalResourceSource(
            string serviceDefinitionPath,
            string roleName,
            string basePath,
            Func<string, RoleEnvironmentException> exceptionFunc)
        {
            _exceptionFunc = exceptionFunc;

            var xDocument = XDocument.Load(serviceDefinitionPath);

            XNamespace serviceDefinitionNamespace = xDocument.Root
                .Attributes()
                .Where(a => a.IsNamespaceDeclaration)
                .First(a => a.Value.Contains("ServiceDefinition"))
                .Value;

            var roleElement = xDocument.Root.Elements()
                .First(e => e.Attribute("name").Value == roleName);

            var localResourcesElement = roleElement.Descendants(serviceDefinitionNamespace + "LocalResources")
                .FirstOrDefault();

            if (localResourcesElement == null)
            {
                _localResources = new Dictionary<string, StandaloneAzureLocalResource>();
                return;
            }

            var processId = roleName + "-" + Process.GetCurrentProcess().Id.ToString(CultureInfo.InvariantCulture);

            _localResources = localResourcesElement.Descendants()
                .ToDictionary(
                    e => e.Attribute("name").Value,
                    e => new StandaloneAzureLocalResource
                    {
                        Name = e.Attribute("name").Value,
                        MaximumSizeInMegabytes = Int32.Parse(e.Attribute("sizeInMB").Value),
                        RootPath = Path.Combine(basePath, ".resources", processId, e.Attribute("name").Value)
                    });
        }

        public IAzureLocalResource this[string index]
        {
            get
            {
                if (!_localResources.ContainsKey(index))
                {
                    throw _exceptionFunc(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown resource '{0}'",
                        index));
                }
                return _localResources[index];
            }
        }
    }
}