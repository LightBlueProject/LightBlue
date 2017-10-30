using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;
using Topshelf;

namespace LightBlue.WorkerService
{
    internal class WorkerHost
    {
        private readonly Settings _settings;
        private RoleEntryPoint _role;
        private HostControl _hostControl;

        public WorkerHost(Settings settings)
        {
            _settings = settings;
        }

        public bool Start(HostControl hc)
        {
            _hostControl = hc;

            var hostDirectory = LightBlueConfiguration.SetAsWindowsHost(_settings.ServiceTitle,
                _settings.ConfigurationPath,
                _settings.RoleName);

            Trace.TraceInformation("Worker host service LightBlue context created at directory {0}", hostDirectory);

            var assemblyPath = Path.IsPathRooted(_settings.Assembly)
                ? _settings.Assembly
                : Path.Combine(Environment.CurrentDirectory, _settings.Assembly);
            var entryPoint = Assembly.LoadFrom(assemblyPath)
                .GetTypes()
                .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
            _role = (RoleEntryPoint)Activator.CreateInstance(entryPoint);

            Trace.TraceInformation("Worker host service role entry point {0} located at {1}", entryPoint.FullName, assemblyPath);

            if (!_role.OnStart())
            {
                Trace.TraceError("Worker host service role entry point {0} start failed", entryPoint.FullName);
                _hostControl.Restart();
                return true;
            }

            Task.Run(() =>
            {
                try
                {
                    _role.Run();
                    Environment.Exit(0);
                }
                catch (Exception)
                {
                    Environment.Exit(1);
                }
            });

            Trace.TraceInformation("Worker host service role entry point {0} running", entryPoint.FullName);

            return true;
        }

        public void Stop()
        {
            _role.OnStop();
            Trace.TraceInformation("Worker host service restarting");
        }
    }
}