using System.IO;
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
            using (var fs = new FileStream(path, FileMode.Truncate, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonConvert.SerializeObject(multiHostConfiguration, Formatting.Indented));
            }
        }
    }
}