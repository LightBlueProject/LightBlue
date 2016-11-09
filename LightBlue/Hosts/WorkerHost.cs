using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Infrastructure;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;
using Serilog;
using Serilog.Context;

namespace LightBlue.Hosts
{
    public class WorkerHost
    {
        private readonly Settings _settings;
        private RoleEntryPoint _role;
        private IDisposable _context;

        public WorkerHost(Settings settings)
        {
            _settings = settings;
        }

        public bool Start()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.RollingFile(LightBlueFileSystem.LocalAppData.FullName + "\\" + "LightBlue-" + _settings.ServiceTitle + "-{Date}.txt", 
                    outputTemplate: "{Timestamp:u} [{Level}] [{Service}] {Message}{NewLine}{Exception}")
                    .Enrich.FromLogContext()
                    .CreateLogger();
                _context = LogContext.PushProperty("Service", _settings.ServiceTitle);

                Log.Information("Starting with settings {@WorkerHostSettings}", _settings);

                LightBlueConfiguration.SetAsLightBlue(_settings.Cscfg,
                    _settings.Csdef,
                    _settings.RoleName,
                    LightBlueHostType.Direct,
                    false);

                var assemblyPath = Path.IsPathRooted(_settings.Assembly)
                    ? _settings.Assembly
                    : Path.Combine(Environment.CurrentDirectory, _settings.Assembly);
                var entryPoint = Assembly.LoadFrom(assemblyPath)
                    .GetTypes()
                    .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
                _role = (RoleEntryPoint)Activator.CreateInstance(entryPoint);

                Log.Information("Role entry point {RoleEntryPoint} located at {AssemblyPath}", entryPoint.FullName, assemblyPath);

                if (!_role.OnStart())
                {
                    Log.Error("Role entry point {RoleEntryPoint} on start failed", entryPoint.FullName);
                    return false;
                }

                Task.Run(() => _role.Run());

                Log.Information("Role entry point {RoleEntryPoint} running", entryPoint.FullName);

                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception {Exception}", ex.GetType().Name);
                return false;
            }
        }

        public bool Stop()
        {
            _role.OnStop();
            Log.Information("Worker service disposed");
            _context.Dispose();
            return true;
        }

        public class Settings
        {
            public string Assembly { get; set; }
            public string RoleName { get; set; }
            public string ServiceTitle { get; set; }
            public string Configuration { get; set; }
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