namespace LightBlue.MultiHost.Configuration
{
    public class RoleConfiguration
    {
        // required by all roles (worker & web)
        public bool EnabledOnStartup { get; set; }
        public string Assembly { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }
        public string ConfigurationPath { get; set; }

        // optional - required for web roles
        public string Port { get; set; }
        public string UseSsl { get; set; }
        public string Hostname { get; set; }

        // optional for all roles
        public string RoleIsolationMode { get; set; }
    }
}
