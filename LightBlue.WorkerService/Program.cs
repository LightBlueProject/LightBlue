using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;
using Topshelf;
using Topshelf.Logging;

namespace LightBlue.WorkerService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                Trace.Listeners.Add(new TopshelfConsoleTraceListener());

                var settings = WorkerHost.Settings.Load();

                x.Service<WorkerHost>(service =>
                {
                    service.ConstructUsing(s => new WorkerHost(settings));
                    service.WhenStarted((s, hc) => s.Start(hc));
                    service.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(string.Format("LightBlue {0} WorkerRole Windows Service", settings.ServiceTitle));
                x.SetDisplayName(settings.ServiceTitle + " Service");
                x.SetServiceName(settings.ServiceTitle);
                x.StartAutomatically();
                x.EnableShutdown();
                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(1);
                });
            });
        }

        public class WorkerHost
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
                    _hostControl.Restart();
                    return true;
                }

                Task.Run(() =>
                {
                    try
                    {
                        _role.Run();
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError("Worker host service errored with exception: {0}", ex);
                        _hostControl.Restart();
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
}
