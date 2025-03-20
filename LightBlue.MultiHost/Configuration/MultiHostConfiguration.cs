using System.Collections.Generic;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfiguration
    {
        // AssemblyCacheId is used to create an isolated cache for each system which allows for MultiHost to run multiple systems on the same machine
        public string AssemblyCacheId { get; set; } = "LightBlue.MultiHost";
        public IEnumerable<RoleConfiguration> Roles { get; set; }
        public int ThreadDelayMs { get; set; } = 50;
        public int AppDomainDelayMs { get; set; } = 50;
        public int ProcessDelayMs { get; set; } = 500;  // longer delay for process isolation

        public MultiHostConfiguration()
        {
            Roles = new RoleConfiguration[0];
        }
    }
}