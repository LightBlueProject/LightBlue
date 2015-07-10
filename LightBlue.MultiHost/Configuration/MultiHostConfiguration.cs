using System.Collections.Generic;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfiguration
    {
        public IEnumerable<RoleConfiguration> Roles { get; set; }

        public MultiHostConfiguration()
        {
            Roles = new RoleConfiguration[0];
        }
    }
}