using System;
using System.Collections.Generic;
using System.IO;

using LightBlue.Infrastructure;

using NDesk.Options;

namespace LightBlue.Host
{
    public class HostArgs
    {
        public string Assembly { get; private set; }
        public string RoleName { get; private set; }
        public string Title { get; private set; }
        public string ConfigurationPath { get; private set; }
        public string ServiceDefinitionPath { get; private set; }
        public string ApplicationBase { get { return Path.GetDirectoryName(Assembly); } }

        public static HostArgs ParseArgs(IEnumerable<string> args)
        {
            string assembly = null;
            string roleName = null;
            string title = null;
            string configurationPath = null;

            var options = new OptionSet
            {
                {"a|assembly=", "", v => assembly = v},
                {"n|roleName=", "", v => roleName = v},
                {"t|serviceTitle=", "", v => title = v},
                {"c|configurationPath=", "", v => configurationPath = v},
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

            return new HostArgs
            {
                Assembly = assembly,
                RoleName = roleName,
                Title = string.IsNullOrWhiteSpace(title)
                    ? roleName
                    : title,
                ConfigurationPath = ConfigurationLocator.LocateConfigurationFile(configurationPath),
                ServiceDefinitionPath = ConfigurationLocator.LocateServiceDefinition(configurationPath),
            };
        }
    }
}