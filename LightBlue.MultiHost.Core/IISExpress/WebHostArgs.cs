using System.IO;

namespace LightBlue.MultiHost.Core.IISExpress
{
    public class WebHostArgs
    {
        public string Assembly { get; set; }
        public int Port { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }
        public string ConfigurationPath { get; set; }
        public string ServiceDefinitionPath { get; set; }
        public bool UseSsl { get; set; }
        public string Hostname { get; set; }
        public bool UseHostedStorage { get; set; }
        public string IisExpressTemplate { get; set; }
        public bool Use64Bit { get; set; }

        public string SiteDirectory
        {
            get { return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly) ?? "", "..")); }
        }

        public string SiteBinDirectory
        {
            get { return Path.GetDirectoryName(Assembly) ?? ""; }
        }
    }
}