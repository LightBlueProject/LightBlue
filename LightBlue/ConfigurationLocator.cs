using System;
using System.IO;

namespace LightBlue
{
    // This is temporary
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
                throw new ArgumentException(
                    "The configuration path does not exist. Specify the specific configuration file or the directory in which ServiceConfiguration.Local.cscfg is located.");
            }

            var configurationFile = Path.Combine(path, "ServiceConfiguration.Local.cscfg");
            if (!File.Exists(configurationFile))
            {
                throw new ArgumentException(
                    "ServiceConfiguration.Local.cscfg cannot be located in the configuration path.");
            }

            return configurationFile;
        }

        private static string RemoveTrailingDoubleQuote(string configurationPath)
        {
            return configurationPath.EndsWith("\"")
                ? configurationPath.Substring(0, configurationPath.Length - 1)
                : configurationPath;
        }
    }
}