using System;
using System.Configuration;
using System.IO;

namespace LightBlue.WorkerService
{
    internal class Settings
    {
        public string Assembly { get; set; }
        public string RoleName { get; set; }
        public string ServiceTitle { get; set; }
        public string ConfigurationPath { get; set; }

        public static Settings Load()
        {
            var settings = new Settings
            {
                Assembly = ConfigurationManager.AppSettings["Assembly"],
                RoleName = ConfigurationManager.AppSettings["RoleName"],
                ServiceTitle = ConfigurationManager.AppSettings["ServiceTitle"],
                ConfigurationPath = ConfigurationManager.AppSettings["ConfigurationPath"],
            };

            if (String.IsNullOrWhiteSpace(settings.Assembly))
                throw new InvalidOperationException("Host requires an assembly to run.");

            if (String.IsNullOrWhiteSpace(settings.RoleName))
                throw new InvalidOperationException("Role Name must be specified.");

            if (String.IsNullOrWhiteSpace(settings.ConfigurationPath))
                throw new InvalidOperationException("Configuration path must be specified.");

            var absoluteAssemblyPath = Path.IsPathRooted(settings.Assembly)
                ? settings.Assembly
                : Path.Combine(Environment.CurrentDirectory, settings.Assembly);

            if (!File.Exists(absoluteAssemblyPath))
                throw new InvalidOperationException("The specified site assembly cannot be found.");

            return settings;
        }
    }
}