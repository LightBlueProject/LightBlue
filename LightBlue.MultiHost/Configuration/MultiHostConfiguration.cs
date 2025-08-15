using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightBlue.MultiHost.Configuration
{
    public class MultiHostConfiguration
    {
        /// <summary>
        /// Jargon:
        /// Service = an instance of proccessing
        /// Role = a type/category of service
        /// Runner = how a service is executed
        /// </summary>
        /// 
        public int ServiceBootRateLimitMs { get; set; } = 10;
        public string AssemblyCacheId { get; set; } = "LightBlue.MultiHost";
        public IEnumerable<CustomRunnerConfiguration> CustomRunners { get; set; } = new List<CustomRunnerConfiguration>();
        // AssemblyCacheId is used to create an isolated cache for each system which allows for MultiHost to run multiple systems on the same machine
        public IEnumerable<RoleConfiguration> RoleConfiguration { get; set; }
        public IEnumerable<ServiceConfiguration> Services { get; set; }

        public MultiHostConfiguration()
        {
            Services = new ServiceConfiguration[0];
        }

        public void Validate()
        {
            var errors = new List<string>();

            if (CustomRunners != null)
            {
                //CustomRunners must be distinct
                if (CustomRunners.Select(x => x.RunnerName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != CustomRunners.Count())
                    errors.Add("Multiple Custom Runners with the same identifier");
            }

            if (RoleConfiguration != null)
            {
                //RoleConfigurations must be distinct
                if (RoleConfiguration.Select(x => x.RoleName).Distinct(StringComparer.OrdinalIgnoreCase).Count() != RoleConfiguration.Count())
                    errors.Add("Multiple Role Configurations with the same identifier");
            }

            foreach (var service in Services)
            {
                var serviceErrors = service.Validate();
                errors.AddRange(serviceErrors);
                //All services with custom RunnerType need a matching runner config
                if (service.RunnerType == RunnerType.Custom)
                {
                    if (CustomRunners == null || !CustomRunners.Any(x => x.RunnerName.Equals(service.RunnerName, StringComparison.OrdinalIgnoreCase)))
                    {
                        errors.Add($"Service {service.Title}, Runner {service.RunnerName}, Runner Configuration is Missing");
                    }
                }
                //All Roles in all services need a matching Role Configuration
                if (RoleConfiguration == null || !RoleConfiguration.Any(x => x.RoleName.Equals(service.RoleName, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add($"Service {service.Title}, Role {service.RoleName}, Role Configuration is Missing");
                }
            }

            if (errors.Any())
            {
                var builder = new StringBuilder();
                builder.AppendLine("Errors:");
                foreach (var error in errors)
                    builder.AppendLine(error);
                throw new ArgumentException($"Host Configured Incorrectly {builder}");
            }
        }
    }
}