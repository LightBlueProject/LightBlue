using System.Text.Json.Serialization;

namespace LightBlue.MultiHost.Configuration
{
    public class RoleConfiguration
    {
        public string RoleName { get; set; }
        [JsonConverter(typeof(IconOptionConverter))]
        public IconOption Icon { get; set; } = IconOption.Worker;
    }
}
