using System.Collections.Generic;

namespace LightBlue.MultiHost.Configuration
{
    public static class IconHelper
    {
        public static readonly IReadOnlyDictionary<IconOption, string> IconPaths = new Dictionary<IconOption, string>()
        {
            [IconOption.Website] = @"Resources\website.ico",
            [IconOption.ReadModelPopulator] = @"Resources\readmodelpopulator.ico",
            [IconOption.ProcessManager] = @"Resources\processmanager.ico",
            [IconOption.MessageHub] = @"Resources\messagehub.ico",
            [IconOption.DomainService] = @"Resources\domainservice.ico",
            [IconOption.Debug] = @"Resources\debug.ico",
            [IconOption.Worker] = @"Resources\worker.ico"
        };

        public static IconOption RoleToIconOption(RoleConfiguration config)
        {
            switch (config.RoleName)
            {
                case "ReadModelPopulator":
                    return IconOption.ReadModelPopulator;
                case "CommandProcessor":
                    return IconOption.DomainService;
                case "ProcessManager":
                    return IconOption.ProcessManager;
                case "AzureFunction":
                case "Npm":
                    return IconOption.Website;
                case "WebRole":
                    return config.Title.Contains("Hub") 
                    ? IconOption.MessageHub
                    : IconOption.Website;
                default:
                    return IconOption.Worker;            
            }
        }
    }
}
