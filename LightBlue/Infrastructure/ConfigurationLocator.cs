using System;
using System.IO;

namespace LightBlue.Infrastructure
{
    public static class ConfigurationLocator
    {
        public static string LocateConfigurationFile(string configurationPath)
        {
            var path = RemoveTrailingDoubleQuote(configurationPath);

            if (File.Exists(path))
            {
                return path;
            }

            if (!Directory.Exists(path))
            {
                throw new ArgumentException("The configuration path does not exist. Specify the specific configuration file or the directory in which ServiceConfiguration.Local.cscfg is located.");
            }

            var configurationFile = Path.Combine(path, "ServiceConfiguration.Local.cscfg");
            if (!File.Exists(configurationFile))
            {
                throw new ArgumentException("ServiceConfiguration.Local.cscfg cannot be located in the configuration path.");
            }

            return configurationFile;
        }

        public static string LocateServiceDefinition(string configurationPath)
        {
            var path = RemoveTrailingDoubleQuote(configurationPath);

            var directoryPath = File.Exists(path)
                ? Path.GetDirectoryName(path) ?? ""
                : path;

            if (!Directory.Exists(directoryPath))
            {
                throw new ArgumentException("The configuration path does not exist. Specify the specific configuration file or the directory in which ServiceConfiguration.Local.cscfg is located.");
            }

            var serviceDefinitionPath = Path.Combine(directoryPath, "ServiceDefinition.csdef");

            if (!File.Exists(serviceDefinitionPath))
            {
                throw new ArgumentException("Cannot locate the ServiceDefinition.csdef file. This must be in the configuration directory. If a cscfg file is specified ServiceDefinition.csdef must be in the same directory as this file.");
            }

            return serviceDefinitionPath;
        }

        private static string RemoveTrailingDoubleQuote(string configurationPath)
        {
            return configurationPath.EndsWith("\"")
                ? configurationPath.Substring(0, configurationPath.Length - 1)
                : configurationPath;
        }
    }
}