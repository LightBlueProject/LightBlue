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

            _settings = xDocument
                .XPathSelectElements("./configuration/appSettings/add")
                .ToDictionary(s => s.Attribute("key").Value, s => s.Attribute("value").Value);
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