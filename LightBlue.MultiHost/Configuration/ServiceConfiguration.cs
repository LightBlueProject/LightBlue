using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LightBlue.MultiHost.Configuration
{
    public class ServiceConfiguration
    {
        // required by all services (worker & web)
        public bool EnabledOnStartup { get; set; }
        public string Assembly { get; set; }
        public string RoleName { get; set; }
        public string Title { get; set; }
        public string ConfigurationPath { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RunnerType RunnerType { get; set; }


        // optional - required for web roles
        public string Port { get; set; }
        public string UseSsl { get; set; }
        public string Hostname { get; set; }

        // optional for all roles
        public string ProcessPriority { get; set; }

        //optional - for custom runners
        public string RunnerName { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Assembly))
                errors.Add("Assembly is missing");
            if (string.IsNullOrWhiteSpace(RoleName))
                errors.Add("RoleName is missing");
            if (string.IsNullOrWhiteSpace(Title))
                errors.Add("Title is missing");
            if (string.IsNullOrWhiteSpace(ConfigurationPath))
                errors.Add("ConfigurationPath is missing");
            if (RunnerType == RunnerType.Unknown)
                errors.Add("Unknown RunnerType");
            //Iis runners need the optionals
            if (RunnerType == RunnerType.IisExpress)
            {
                if (string.IsNullOrWhiteSpace(Port))
                    errors.Add("Port is missing for IISRunner");
                if (string.IsNullOrWhiteSpace(UseSsl))
                    errors.Add("UseSsl is missing for IISRunner");
                if (string.IsNullOrWhiteSpace(Hostname))
                    errors.Add("Hostname is missing for IISRunner");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Port))
                    errors.Add("Port can only be used with IISRunner");
                if (!string.IsNullOrWhiteSpace(UseSsl))
                    errors.Add("UseSsl can only be used with IISRunner");
                if (!string.IsNullOrWhiteSpace(Hostname))
                    errors.Add("Hostname can only be used with IISRunner");
            }

            //Custom Runners needs a Runner Name
            if (RunnerType == RunnerType.Custom)
            {
                if (string.IsNullOrWhiteSpace(RunnerName))
                    errors.Add("RunnerName is missing for CustomRunner");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(RunnerName))
                    errors.Add("RunnerName can only be used with CustomRunner");
            }

            return errors;
        }
    }
}
