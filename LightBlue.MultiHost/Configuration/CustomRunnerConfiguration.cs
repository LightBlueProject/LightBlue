using System.Text.Json.Serialization;

namespace LightBlue.MultiHost.Configuration
{
    public class CustomRunnerConfiguration
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public string Arguments { get; set; } = string.Empty;
        [JsonConverter(typeof(IconOptionConverter))]
        public IconOption Icon { get; set; } = IconOption.Worker;
    }
}
