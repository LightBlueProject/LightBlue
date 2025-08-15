using System.IO;
using System.Text.Json;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfigurationService
    {
        public MultiHostConfiguration Load(string path)
        {
            var jsonText = File.ReadAllText(path);
            var configuration = JsonSerializer.Deserialize<MultiHostConfiguration>(jsonText, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            });

            configuration.Validate();

            return configuration;
        }

        public void Save(string path, MultiHostConfiguration multiHostConfiguration)
        {
            using (var fs = new FileStream(path, FileMode.Truncate, FileAccess.Write))
            using (var sw = new StreamWriter(fs))
            {
                sw.WriteLine(JsonSerializer.Serialize(multiHostConfiguration, new JsonSerializerOptions { WriteIndented = true, IgnoreNullValues = true }));
            }
        }
    }
}