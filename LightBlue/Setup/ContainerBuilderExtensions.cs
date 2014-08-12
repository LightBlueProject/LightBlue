using Autofac;

using LightBlue.Infrastructure;

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
    }
}