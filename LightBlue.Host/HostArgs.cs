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
        public bool UseHostedStorage { get; private set; }
        public string ApplicationBase { get { return Path.GetDirectoryName(Assembly); } }

        public static HostArgs ParseArgs(IEnumerable<string> args)
        {
            string assembly = null;
            string roleName = null;
            string title = null;
            string configurationPath = null;
            var useHostedStorage = false;
            var displayHelp = false;

            var options = new OptionSet
            {
                {
                    "a|assembly=",
                    "The path to the primary {ASSEMBLY} for the worker role.",
                    v => assembly = v
                },
                {
                    "n|roleName=",
                    "The {NAME} of the role as defined in the configuration file.",
                    v => roleName = v
                },
                {
                    "t|serviceTitle=",
                    "Optional {TITLE} for the role window. Defaults to role name if not specified.",
                    v => title = v
                },
                {
                    "c|configurationPath=",
                    "The {PATH} to the configuration file. Either the directory containing ServiceConfiguration.Local.cscfg or the path to a specific alternate .cscfg file.",
                    v => configurationPath = v
                },
                {
                    "useHostedStorage",
                    "Use hosted storage (Emulator/Actual Azure) inside the LightBlue host.",
                    v => useHostedStorage = true
                },
                {
                    "help",
                    "Show this message and exit.",
                    v => displayHelp = true 
                }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                DisplayErrorMessage(e.Message);
                return null;
            }

            if (displayHelp)
            {
                DisplayHelp(options);
                return null;
            }

            if (string.IsNullOrWhiteSpace(assembly))
            {
                DisplayErrorMessage("Host requires an assembly to run.");
                return null;
            }
            if (string.IsNullOrWhiteSpace(roleName))
            {
                DisplayErrorMessage("Role Name must be specified.");
                return null;
            }
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                DisplayErrorMessage("Configuration path must be specified.");
                return null;
            }

            var roleAssemblyAbsolutePath = Path.IsPathRooted(assembly)
                ? assembly
                : Path.Combine(Environment.CurrentDirectory, assembly);

            if (!File.Exists(roleAssemblyAbsolutePath))
            {
                DisplayErrorMessage("The specified assembly cannot be found. The assembly must be in the host directory or be specified as an absolute path.");
                return null;
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
                UseHostedStorage = useHostedStorage
            };
        }

        private static void DisplayErrorMessage(string message)
        {
            Console.Write("LightBlue.Host: ");
            Console.WriteLine(message);
            Console.WriteLine("Try `LightBlue.Host --help' for more information.");
        }

        static void DisplayHelp(OptionSet p)
        {
            Console.WriteLine("Usage: LightBlue.Host [OPTIONS]");
            Console.WriteLine("Host a LightBlue based Azure worker role.");
            Console.WriteLine();
            Console.WriteLine("Options:");
            p.WriteOptionDescriptions(Console.Out);
        }
    }
}