using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Hosts
{
    public class WorkerHost
    {
        private readonly Settings _settings;
        private RoleEntryPoint _role;
        private Task _task;

        public WorkerHost(Settings settings)
        {
            _settings = settings;
        }

        public void Start()
        {
            var hostDirectory = LightBlueConfiguration.SetAsWindowsHost(_settings.ServiceTitle,
                _settings.Cscfg,
                _settings.Csdef,
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
                throw new InvalidOperationException("Failed to start role entry point " + entryPoint.FullName);
            }

            _task = Task.Run(() =>
            {
                try
                {
                    _role.Run();
                }
                catch (Exception ex)
                {
                    if (_role != null)
                        _role.OnStop();

                    Trace.TraceError("Worker role errored: {0}", ex.Message);

                    throw;
                }
            });

            Trace.TraceInformation("Worker host service role entry point {0} running", entryPoint.FullName);
        }

        public void Stop()
        {
            _role.OnStop();

            Trace.TraceInformation("Worker host service disposed");
        }

        public class Settings
        {
            public string Assembly { get; set; }
            public string RoleName { get; set; }
            public string ServiceTitle { get; set; }
            public string Cscfg { get; set; }
            public string Csdef { get; set; }

            public static Settings Load()
            {
                var settings = new Settings
                {
                    Assembly = ConfigurationManager.AppSettings["Assembly"],
                    RoleName = ConfigurationManager.AppSettings["RoleName"],
                    ServiceTitle = ConfigurationManager.AppSettings["ServiceTitle"],
                    Cscfg = ConfigurationManager.AppSettings["Cscfg"],
                    Csdef = ConfigurationManager.AppSettings["Csdef"]
                };

                if (string.IsNullOrWhiteSpace(settings.Assembly))
                    throw new InvalidOperationException("Host requires an assembly to run.");

                if (string.IsNullOrWhiteSpace(settings.RoleName))
                    throw new InvalidOperationException("Role Name must be specified.");

                if (string.IsNullOrWhiteSpace(settings.Cscfg))
                    throw new InvalidOperationException("Cscfg path must be specified.");

                if (string.IsNullOrWhiteSpace(settings.Csdef))
                    throw new InvalidOperationException("Csdef path must be specified.");

                var absoluteAssemblyPath = Path.IsPathRooted(settings.Assembly)
                    ? settings.Assembly
                    : Path.Combine(Environment.CurrentDirectory, settings.Assembly);

                if (!File.Exists(absoluteAssemblyPath))
                    throw new InvalidOperationException("The specified site assembly cannot be found.");

                return settings;
            }
        }
    }
}