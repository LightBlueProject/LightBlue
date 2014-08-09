using System;
using System.Collections.Generic;
using System.IO;

using NDesk.Options;

namespace LightBlue.Host
{
    public class HostArgs
    {
        public string Assembly { get; private set; }
        public string RoleName { get; private set; }
        public string ConfigurationPath { get; private set; }
        public RetryMode RetryMode { get; private set; }

        public static HostArgs ParseArgs(IEnumerable<string> args)
        {
            string assembly = null;
            string roleName = null;
            string configurationPath = null;
            var retryMode = RetryMode.Infinite;

            var options = new OptionSet
            {
                {"a|assembly=", "", v => assembly = v},
                {"n|roleName=", "", v => roleName = v},
                {"c|configurationPath=", "", v => configurationPath = v},
                {"r|retryMode=", "", v => { retryMode = (RetryMode) Enum.Parse(typeof(RetryMode), v, true); }},
            };

            options.Parse(args);

            if (string.IsNullOrWhiteSpace(assembly))
            {
                throw new ArgumentException("Host requires an assembly to run.");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Role Name must be specified.");
            }
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                throw new ArgumentException("Configuration path must be specified.");
            }

            var roleAssemblyAbsolutePath = Path.IsPathRooted(assembly)
                ? assembly
                : Path.Combine(Environment.CurrentDirectory, assembly);

            if (!File.Exists(roleAssemblyAbsolutePath))
            {
                throw new FileNotFoundException("The specified assembly cannot be found. The assembly must be in the host directory or be specified as an absolute path.");
            }

            var configurationFile = LocateConfigurationFile(configurationPath);

            return new HostArgs
            {
                Assembly = assembly,
                RoleName = roleName,
                ConfigurationPath = configurationFile,
                RetryMode = retryMode
            };
        }

        private static string LocateConfigurationFile(string configurationPath)
        {
            if (File.Exists(configurationPath))
            {
                return configurationPath;
            }

            if (!Directory.Exists(configurationPath))
            {
                throw new ArgumentException("The configuration path does not exist. Specify the specific configuration file or the directory in which ServiceConfiguration.Local.cscfg is located.");
            }

            var configurationFile = Path.Combine(configurationPath, "ServiceConfiguration.Local.cscfg");
            if (!File.Exists(configurationFile))
            {
                throw new ArgumentException("ServiceConfiguration.Local.cscfg cannot be located in the configuration path.");
            }

            return configurationFile;
        }
    }
}