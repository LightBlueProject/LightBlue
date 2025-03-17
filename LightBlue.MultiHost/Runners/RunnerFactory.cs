using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LightBlue.MultiHost.ViewModel;

namespace LightBlue.MultiHost.Runners
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

        public static IRunner CreateForWorkerRole(Role role, RoleIsolationMode isolation)
        {
            var config = role.Config;

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
                    return new ThreadRunner(role, assemblyFilePath, ConfigurationLocator.LocateConfigurationFile(config.ConfigurationPath), role.RoleName);
                case RoleIsolationMode.AppDomain:
                    var setup = new AppDomainSetup
                    {
                        ApplicationBase = assemblyPath,
                        ConfigurationFile = config.Assembly + ".config",
                    };
                    return new AppDomainRunner(role, setup, assemblyFilePath, ConfigurationLocator.LocateConfigurationFile(config.ConfigurationPath), role.RoleName);
                case RoleIsolationMode.Process:
                    return new DotNetFrameworkRunner(role);
                default:
                    throw new NotSupportedException();
            }
        }

        public static IRunner CreateDotNetCoreRunner(Role role)
        {
            return new DotNetCoreRunner(role);
        }

        public static IRunner CreateDotNetFrameworkRunner(Role role)
        {
            return new DotNetFrameworkRunner(role);
        }

        public static IRunner CreateNpmRunner(Role role)
        {
            return new NpmRunner(role);
        }

        public static IRunner CreateAzureFunctionRunner(Role role)
        {
            return new AzureFunctionRunner(role);
        }
    }
}