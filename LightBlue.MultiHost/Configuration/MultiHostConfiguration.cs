using System.Collections.Generic;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfiguration
    {
        // SystemName is used to create an isolated cache for each system which allows for MultiHost to run multiple systems on the same machine
        // This does not currently extend to isolating any other resources, such as Blob Storage
        public string SystemName { get; set; } = "LightBlue.MultiHost";
        public IEnumerable<RoleConfiguration> Roles { get; set; }

        public MultiHostConfiguration()
        {
            Roles = new RoleConfiguration[0];
        }
    }
}