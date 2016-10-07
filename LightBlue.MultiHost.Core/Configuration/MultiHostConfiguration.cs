using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LightBlue.MultiHost.Core.Configuration
{
    public class MultiHostConfiguration
    {
        public static MultiHostConfiguration Default = new MultiHostConfiguration
        {
            File = null,
            Roles = new[]
            {
                new RoleConfiguration
                {
                    Title = "Demo Web Site",
                    RoleName = "WebRole"
                },
                new RoleConfiguration
                {
                    Title = "Demo Web Site 2",
                    RoleName = "WebRole",
                    RoleIsolationMode = "AppDomain"
                },
                new RoleConfiguration
                {
                    Title = "Demo Domain",
                    RoleName = "CommandProcessor"
                },
                new RoleConfiguration
                {
                    Title = "Demo Domain 2",
                    RoleName = "ReadModelPopulator",
                    RoleIsolationMode = "AppDomain"
                }
            }
        };

        public FileInfo File { get; set; }

        public IEnumerable<RoleConfiguration> Roles { get; set; }

        public static MultiHostConfiguration Load(string path)
        {
            var file = new FileInfo(path);
            if (!file.Exists)
                throw new FileNotFoundException("Failed to load multhost configuration json file", path);

            var jsonText = System.IO.File.ReadAllText(file.FullName);
            var configuration = JsonConvert.DeserializeObject<MultiHostConfiguration>(jsonText);

            foreach (var c in configuration.Roles)
            {
                c.ConfigurationPath = Path.GetFullPath(Path.Combine(file.DirectoryName, c.ConfigurationPath));
                c.Assembly = Path.GetFullPath(Path.Combine(file.DirectoryName, c.Assembly));
            }

            configuration.File = file;

            return configuration;
        }

        public string[] GetAssemblyLocations()
        {
            var relativePaths =
                from r in Roles
                let relativePath = r.Assembly.ToLowerInvariant().EndsWith(".dll")
                    ? Path.GetDirectoryName(r.Assembly)
                    : r.Assembly
                select relativePath;
            return relativePaths.ToArray();
        }
    }
}