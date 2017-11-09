using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Topshelf;

namespace LightBlue.WorkerService
{
    internal class WorkerHost
    {
        private readonly Settings _settings;

        public WorkerHost(Settings settings)
        {
            _settings = settings;
        }

        public bool Start(HostControl hc)
        {
            var hostDirectory = LightBlueConfiguration.SetAsWindowsHost(_settings.ServiceTitle,
                _settings.ConfigurationPath,
                _settings.RoleName);

            Trace.TraceInformation("Worker host service LightBlue context created at directory {0}", hostDirectory);

            var assemblyPath = Path.IsPathRooted(_settings.Assembly)
                ? _settings.Assembly
                : Path.Combine(Environment.CurrentDirectory, _settings.Assembly);

            return RunConsoleApplication(assemblyPath);
        }

        private static bool RunConsoleApplication(string assemblyPath)
        {
            var entryPoint = Assembly.LoadFrom(assemblyPath).EntryPoint;

            Trace.TraceInformation("Worker host service entry point {0} located at {1}", entryPoint.Name, assemblyPath);

            Task.Run(() =>
            {
                try
                {
                    entryPoint.Invoke(null, new object[] {null});
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    Environment.Exit(1);
                }
            });

            Trace.TraceInformation("Worker host service entry point {0} running", entryPoint.Name);

            return true;
        }
    }
}