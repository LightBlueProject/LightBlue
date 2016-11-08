using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;
using Microsoft.WindowsAzure.ServiceRuntime;
using Serilog;
using Serilog.Context;
using System.Configuration;
using LightBlue.Infrastructure;

namespace LightBlue.Hosts
{
    public class WebHost
    {
        private readonly Settings _settings;
        private RoleEntryPoint _role;
        private ProcessHost _iis;
        private IDisposable _context;
        private Task _task;

        public bool IsDisposed { get; set; }

        public WebHost(Settings settings)
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

                Log.Information("Starting with settings {@WebHostSettings}", _settings);

                LightBlueConfiguration.SetAsLightBlue(_settings.Cscfg,
                    _settings.Csdef,
                    _settings.RoleName,
                    LightBlueHostType.Direct,
                    false);

                _iis = IISExpress.Start(_settings, Log.Information);
                _task = Task.Run(() =>
                {
                    _iis.Start();
                });

                var entryPoint = Assembly.LoadFrom(_settings.Assembly)
                    .GetTypes()
                    .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
                _role = (RoleEntryPoint)Activator.CreateInstance(entryPoint);

                Log.Information("Role entry point {RoleEntryPoint} located at {AssemblyLocation}", entryPoint.FullName, entryPoint.Assembly.Location);

                if (!_role.OnStart())
                {
                    Log.Error("Role entry point {RoleEntryPoint} on start failed", entryPoint.FullName);
                    return false;
                }

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
            if (IsDisposed)
                return true;

            _role.OnStop();
            _iis.Dispose();
            _task.Dispose();

            Log.Information("Web service disposed");
            _context.Dispose();

            IsDisposed = true;
            return true;
        }

        public class Settings
        {
            public string Assembly { get; set; }
            public string Port { get; set; }
            public string RoleName { get; set; }
            public string ServiceTitle { get; set; }
            public string Cscfg { get; set; }
            public string Csdef { get; set; }
            public bool UseSSL { get; set; }
            public string Host { get; set; }

            public string SiteDirectory
            {
                get { return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly) ?? "", "..")); }
            }

            public static Settings Load()
            {
                var settings = new Settings
                {
                    Assembly = ConfigurationManager.AppSettings["Assembly"],
                    Port = ConfigurationManager.AppSettings["Port"],
                    RoleName = ConfigurationManager.AppSettings["RoleName"],
                    ServiceTitle = ConfigurationManager.AppSettings["ServiceTitle"],
                    Cscfg = ConfigurationManager.AppSettings["Cscfg"],
                    Csdef = ConfigurationManager.AppSettings["Csdef"],
                    UseSSL = bool.Parse(ConfigurationManager.AppSettings["UseSSL"]),
                    Host = ConfigurationManager.AppSettings["Host"]
                };

                if (string.IsNullOrWhiteSpace(settings.Assembly))
                    throw new InvalidOperationException("Host requires an assembly to run.");

                if (string.IsNullOrWhiteSpace(settings.RoleName))
                    throw new InvalidOperationException("Role Name must be specified.");

                if (string.IsNullOrWhiteSpace(settings.Cscfg))
                    throw new InvalidOperationException("Configuration path must be specified.");

                if (string.IsNullOrWhiteSpace(settings.Host))
                    throw new InvalidOperationException("The hostname cannot be blank. Do not specify this option if you wish to use the default (localhost).");

                var absoluteAssemblyPath = Path.IsPathRooted(settings.Assembly)
                    ? settings.Assembly
                    : Path.Combine(Environment.CurrentDirectory, settings.Assembly);

                if (!File.Exists(absoluteAssemblyPath))
                    throw new InvalidOperationException("The specified site assembly cannot be found.");

                settings.Assembly = absoluteAssemblyPath;

                return settings;
            }
        }
    }
}