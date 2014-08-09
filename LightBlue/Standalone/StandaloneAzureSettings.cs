using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Standalone
{
    public class StandaloneAzureSettings : IAzureSettings
    {
        private readonly Func<string, RoleEnvironmentException> _exceptionFunc;
        private readonly Dictionary<string, string> _settings;

        public StandaloneAzureSettings(
            string configurationPath,
            string roleName,
            Func<string, RoleEnvironmentException> exceptionFunc)
        {
            _exceptionFunc = exceptionFunc;

            var xDocument = XDocument.Load(configurationPath);

            XNamespace serviceConfigurationNNamespace = xDocument.Root
                .Attributes()
                .Where(a => a.IsNamespaceDeclaration)
                .First(a => a.Value.Contains("ServiceConfiguration"))
                .Value;

            var roleElement = xDocument.Descendants(serviceConfigurationNNamespace + "Role")
                .First(r => r.Attribute("name").Value == roleName);

            _settings = roleElement.Descendants(serviceConfigurationNNamespace + "ConfigurationSettings")
                .Descendants(serviceConfigurationNNamespace + "Setting")
                .ToDictionary(s => s.Attribute("name").Value, s => s.Attribute("value").Value);
        }

        public string this[string index]
        {
            get
            {
                if (!_settings.ContainsKey(index))
                {
                    throw _exceptionFunc(string.Format(
                        CultureInfo.InvariantCulture,
                        "Unknown setting '{0}'",
                        index));
                }

                return _settings[index];
            }
        }
    }
}