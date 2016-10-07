using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBlue.Infrastructure;
using LightBlue.MultiHost.Core.IISExpress;

namespace LightBlue.MultiHost.Core.Runners
{
    class RunnerFactory
    {
        private readonly static object Gate = new object();
        private readonly static Dictionary<string, DateTime> Dictionary = new Dictionary<string, DateTime>();

        public static void TrackDirectory(string assemblyPath)
        {
            lock (Gate)
            {
                Dictionary.Add(assemblyPath, DateTime.Now);
            }
        }

        public static bool HasBeenReBuilt(string assemblyPath)
        {
            DateTime date;
            bool containsKey;
            lock (Gate)
            {
                containsKey = Dictionary.TryGetValue(assemblyPath, out date);
            }
            if (containsKey)
            {
                var max =
                    Directory.EnumerateFiles(assemblyPath)
                        .Select(x => new FileInfo(x).LastWriteTime)
                        .Max();
                if (max > date)
                {
                    return true;
                }
            }
            else
            {
                TrackDirectory(assemblyPath);
            }
            return false;
        }

        public static IRunner CreateForWebSite(Role role)
        {
            return new IisExpressRunner(role);
        }

        public static IRunner CreateForWebRole(Role role, RoleIsolationMode isolation)
        {
            var args = WebConfigHelper.Create(role.Config);


            var assemblyPath = Path.GetDirectoryName(args.Assembly);
            if (HasBeenReBuilt(assemblyPath) && isolation == RoleIsolationMode.Thread)
            {
                isolation = RoleIsolationMode.AppDomain;
                role.IsolationMode = isolation;
            }

            switch (isolation)
            {
                case RoleIsolationMode.Thread:
                    return new ThreadRunner(role, args.Assembly, args.ConfigurationPath, args.ServiceDefinitionPath, args.RoleName);
                case RoleIsolationMode.AppDomain:
                    var setup = new AppDomainSetup
                    {
                        ApplicationBase = args.SiteBinDirectory,
                        ConfigurationFile = WebConfigHelper.DetermineWebConfigPath(args.Assembly)
                    };
                    return new AppDomainRunner(role, setup, args.Assembly, args.ConfigurationPath, args.ServiceDefinitionPath, args.RoleName);
                default:
                    throw new NotSupportedException();
            }
        }

        public static IRunner CreateForWorkerRole(Role role, RoleIsolationMode isolation)
        {
            var config = role.Config;
            var configurationPath = config.ConfigurationPath;
            var configurationFilePath = ConfigurationLocator.LocateConfigurationFile(configurationPath);
            var serviceDefinitionPath = ConfigurationLocator.LocateServiceDefinition(configurationPath);

            var assemblyFilePath = config.Assembly;
            var assemblyPath = Path.GetDirectoryName(assemblyFilePath);

            if (HasBeenReBuilt(assemblyPath) && isolation == RoleIsolationMode.Thread)
            {
                isolation = RoleIsolationMode.AppDomain;
                role.IsolationMode = isolation;
            }

            switch (isolation)
            {
                case RoleIsolationMode.Thread:
                    return new ThreadRunner(role, assemblyFilePath, configurationFilePath, serviceDefinitionPath, role.RoleName);
                case RoleIsolationMode.AppDomain:
                    var setup = new AppDomainSetup
                    {
                        ApplicationBase = assemblyPath,
                        ConfigurationFile = config.Assembly + ".config",
                    };
                    return new AppDomainRunner(role, setup, assemblyFilePath, configurationFilePath, serviceDefinitionPath, role.RoleName);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}