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

            XNamespace serviceConfigurationNamespace = xDocument.Root
                .Attributes()
                .Where(a => a.IsNamespaceDeclaration)
                .First(a => a.Value.Contains("ServiceConfiguration"))
                .Value;

            var roleElement = xDocument.Descendants(serviceConfigurationNamespace + "Role")
                .FirstOrDefault(r => r.Attribute("name").Value == roleName);

            if (roleElement == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Cannot find configuration for the role '{0}' in '{1}'",
                        roleName,
                        configurationPath));
            }

            _settings = roleElement.Descendants(serviceConfigurationNamespace + "ConfigurationSettings")
                .Descendants(serviceConfigurationNamespace + "Setting")
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