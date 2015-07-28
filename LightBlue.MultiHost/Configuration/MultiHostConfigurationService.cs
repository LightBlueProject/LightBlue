using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfigurationService
    {
        public MultiHostConfiguration Load(string path)
        {
            var jsonText = File.ReadAllText(path);
            var configuration = JsonConvert.DeserializeObject<MultiHostConfiguration>(jsonText);

            return configuration;
        }

        

        public void Save(string path, MultiHostConfiguration multiHostConfiguration)
        {
            // This model ensures that we don't persist web related configuration into standard (worker) roles
            var persistanceModel = new
            {
                Roles = from x in multiHostConfiguration.Roles
                        let isWorker = string.IsNullOrWhiteSpace(x.Port)
                        select isWorker
                            ? (dynamic)new
                            {
                                x.EnabledOnStartup,
                                x.Assembly,
                                x.RoleName,
                                x.Title,
                                x.ConfigurationPath,
                                
                                x.RoleIsolationMode,
                            }
                            : (dynamic)new
                            {
                                x.EnabledOnStartup,
                                x.Assembly,
                                x.RoleName,
                                x.Title,
                                x.ConfigurationPath,

                                x.Port,
                                x.UseSsl,
                                x.Hostname,

                                x.RoleIsolationMode,
                            }
            };

            using (var fs = new FileStream(path, FileMode.Truncate, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonConvert.SerializeObject(persistanceModel, Formatting.Indented));
            }
        }
    }
}