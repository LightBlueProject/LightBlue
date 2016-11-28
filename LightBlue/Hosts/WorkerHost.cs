using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;
using Serilog;

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
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.LiterateConsole()
                    .WriteTo.RollingFile(@"c:\LightBlueLogs\" + _settings.ServiceTitle + "-{Date}.txt")
                    .CreateLogger();

                Log.Information("Worker host service current directory {CurrentDirectory}", Directory.GetCurrentDirectory());

                var hostDirectory = LightBlueConfiguration.SetAsWindowsHost(_settings.ServiceTitle,
                    _settings.Cscfg,
                    _settings.Csdef,
                    _settings.RoleName);

                Log.Information("Worker host service LightBlue context created at directory {HostDirectory}", hostDirectory);

                var assemblyPath = Path.IsPathRooted(_settings.Assembly)
                    ? _settings.Assembly
                    : Path.Combine(Environment.CurrentDirectory, _settings.Assembly);
                var entryPoint = Assembly.LoadFrom(assemblyPath)
                    .GetTypes()
                    .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
                _role = (RoleEntryPoint)Activator.CreateInstance(entryPoint);

                Log.Information("Worker host service role entry point {RoleEntryPoint} located at {AssemblyPath}", entryPoint.FullName, assemblyPath);

                if (!_role.OnStart())
                {
                    Log.Error("Worker host service role entry point {EntryPoint} start failed", entryPoint.FullName);
                    throw new InvalidOperationException("Failed to start role entry point " + entryPoint.FullName);
                }

                Log.Information("Worker host service role entry point {EntryPoint} started", entryPoint.FullName);

                _task = Task.Run(() => _role.Run());

                Log.Information("Worker host service role entry point {EntryPoint} running", entryPoint.FullName);
            }
            catch (Exception ex)
            {
                Log.Information("Worker host service errored with exception: ", ex.Message);
                throw;
            }
        }

        public void Stop()
        {
            _role.OnStop();
            _task.Dispose();
            Log.Information("Worker host service disposed");
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