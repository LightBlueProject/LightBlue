using System;

using Autofac;

using LightBlue.Infrastructure;
using LightBlue.Standalone;

namespace LightBlue.Setup
{
    public static class ContainerBuilderExtensions
    {
        public static void RegisterLightBlueModules(this ContainerBuilder builder)
        {
            foreach (var module in ModuleSource.RegisteredModules)
            {
                builder.RegisterModule(module);
            }
        }

        public static void RegisterLightBlueWebModules(this ContainerBuilder builder)
        {
            bool isHosted;

            if (!Boolean.TryParse(Environment.GetEnvironmentVariable("LightBlueHost"), out isHosted))
            {
                isHosted = false;
            }

            if (isHosted)
            {
                ModuleSource.ClearModules();
                ModuleSource.AddModule(new LightBlueStandaloneModule(new StandaloneConfiguration
                {
                    ConfigurationPath = Environment.GetEnvironmentVariable("LightBlueConfigurationPath"),
                    RoleName = Environment.GetEnvironmentVariable("LightBlueRoleName"),
                }));                
            }

            builder.RegisterLightBlueModules();
        }
    }
}