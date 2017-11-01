using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using LightBlue.Setup;

namespace LightBlue.Standalone
{
    public class StandaloneAzureSettings : IAzureSettings
    {
        private readonly Dictionary<string, string> _settings;

        public StandaloneAzureSettings(StandaloneConfiguration standaloneConfiguration)
        {
            var xDocument = XDocument.Load(standaloneConfiguration.ConfigurationPath);

            if (!xDocument.Root.Attributes().Any(x => x.Value.Contains("ServiceConfiguration")))
            {
                _settings = xDocument
                    .XPathSelectElements("./configuration/appSettings/add")
                    .ToDictionary(s => s.Attribute("key").Value, s => s.Attribute("value").Value);
            }
            else
            {
                XNamespace serviceConfigurationNamespace = xDocument.Root
                    .Attributes()
                    .Where(a => a.IsNamespaceDeclaration)
                    .First(a => a.Value.Contains("ServiceConfiguration"))
                    .Value;

                var roleElement = xDocument.Descendants(serviceConfigurationNamespace + "Role")
                    .FirstOrDefault(r => r.Attribute("name").Value == standaloneConfiguration.RoleName);

                if (roleElement == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            "Cannot find configuration for the role '{0}' in '{1}'",
                            standaloneConfiguration.RoleName,
                            standaloneConfiguration.ConfigurationPath));
                }

                _settings = roleElement.Descendants(serviceConfigurationNamespace + "ConfigurationSettings")
                    .Descendants(serviceConfigurationNamespace + "Setting")
                    .ToDictionary(s => s.Attribute("name").Value, s => s.Attribute("value").Value);
            }
        }

        public string this[string index]
        {
            get
            {
                if (!_settings.ContainsKey(index))
                {
                    throw  LightBlueConfiguration.RoleEnvironmentExceptionCreator(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown setting '{0}'",
                        index));
                }

                return _settings[index];
            }
        }
    }
}