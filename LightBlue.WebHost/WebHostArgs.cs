using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using LightBlue.Infrastructure;

using NDesk.Options;

namespace LightBlue.WebHost
{
    public class WebHostArgs
    {
        public string Assembly { get; private set; }
        public int Port { get; private set; }
        public string RoleName { get; private set; }
        public string ConfigurationPath { get; private set; }
        public string ServiceDefinitionPath { get; private set; }
        public bool UseSsl { get; private set; }
        public string Hostname { get; private set; }

        public string SiteDirectory
        {
            get { return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly) ?? "", "..")); }
        }

        public string SiteBinDirectory
        {
            get { return Path.GetDirectoryName(Assembly) ?? ""; }
        }

        public static WebHostArgs ParseArgs(IEnumerable<string> args)
        {
            string assembly = null;
            var port = 0;
            string roleName = null;
            string configurationPath = null;
            bool useSsl = false;
            string hostname = "localhost";

            var options = new OptionSet
            {
                {"a|assembly=", "", v => assembly = v},
                {"p|port=", "", v => Int32.TryParse(v, NumberStyles.None, CultureInfo.InvariantCulture, out port)},
                {"n|roleName=", "", v => roleName = v},
                {"c|configurationPath=", "", v => configurationPath = v},
                {"s|useSsl=", "", v => Boolean.TryParse(v, out useSsl)},
                {"h|hostname=", "", v => hostname = v},
            };

            options.Parse(args);

            if (string.IsNullOrWhiteSpace(assembly))
            {
                throw new ArgumentException("Host requires an assembly to run.");
            }
            if (port == 0)
            {
                throw new ArgumentException("Host requires a port to run the site on");
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException("Role Name must be specified.");
            }
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                throw new ArgumentException("Configuration path must be specified.");
            }
            if (string.IsNullOrWhiteSpace(hostname))
            {
                throw new ArgumentException("Hostname must be specified.");
            }

            var roleAssemblyAbsolutePath = Path.IsPathRooted(assembly)
                ? assembly
                : Path.Combine(Environment.CurrentDirectory, assembly);

            if (!File.Exists(roleAssemblyAbsolutePath))
            {
                throw new FileNotFoundException("The specified site assembly cannot be found.");
            }

            return new WebHostArgs
            {
                Assembly = assembly,
                Port = port,
                RoleName = roleName,
                ConfigurationPath = ConfigurationLocator.LocateConfigurationFile(configurationPath),
                ServiceDefinitionPath = ConfigurationLocator.LocateServiceDefinition(configurationPath),
                UseSsl = useSsl,
                Hostname = hostname
            };
        }
    }
}