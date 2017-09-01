using System;
using System.IO;
using System.Configuration;
using System.Diagnostics;

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
            Trace.TraceInformation("Web host service directory {0}", Directory.GetCurrentDirectory());

            try
            {
                _iis = WebHostProcessFactory.Create(_settings);
                _iis.Start();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Web host service errored with exception: {0}", ex.Message);
                throw;
            }
        }

        public void Stop()
        {
            _iis.Dispose();

            Trace.TraceInformation("Web host service disposed");
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
                    Assembly = ToAbsolutePath(ConfigurationManager.AppSettings["Assembly"]),
                    Port = ConfigurationManager.AppSettings["Port"],
                    RoleName = ConfigurationManager.AppSettings["RoleName"],
                    ServiceTitle = ConfigurationManager.AppSettings["ServiceTitle"],
                    Cscfg = ToAbsolutePath(ConfigurationManager.AppSettings["Cscfg"]),
                    Csdef = ToAbsolutePath(ConfigurationManager.AppSettings["Csdef"]),
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

                return settings;
            }

            private static string ToAbsolutePath(string path)
            {
                var absolutePath = Path.IsPathRooted(path)
                    ? path
                    : Path.Combine(Environment.CurrentDirectory, path);
                if (!File.Exists(absolutePath))
                    throw new FileNotFoundException("File not found at path " + absolutePath);
                return absolutePath;
            }
        }
    }
}