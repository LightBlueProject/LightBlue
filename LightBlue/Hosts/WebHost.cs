using System;
using System.IO;
using Serilog;
using System.Configuration;

namespace LightBlue.Hosts
{
    public class WebHost
    {
        private readonly Settings _settings;
        private WebHostProcess _iis;
        public WebHost(Settings settings)
        {
            _settings = settings;
        }

        public void Start()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile(@"c:\LightBlueLogs\" + _settings.ServiceTitle + "-{Date}.txt")
                .CreateLogger();

            Log.Information("Web host service directory {Directory}", Directory.GetCurrentDirectory());

            try
            {
                _iis = WebHostProcessFactory.Create(_settings);
                _iis.Start();
            }
            catch (Exception ex)
            {
                Log.Information("Web host service errored with exception: ", ex.Message);
                throw;
            }
        }

        public void Stop()
        {
            _iis.Dispose();

            Log.Information("Web host service disposed");
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