using System;
using System.Reflection;

using Autofac;

using LightBlue.Infrastructure;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Standalone
{
    public class LightBlueStandaloneModule : Autofac.Module
    {
        private readonly StandaloneConfiguration _configuration;

        public LightBlueStandaloneModule(StandaloneConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .InNamespace(typeof(LightBlueStandaloneModule).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterType<StandaloneAzureEnvironmentSource>()
                .SingleInstance()
                .As<IAzureEnvironmentSource>();

            builder.RegisterInstance(RoleEnvironmentExceptionCreatorFactory.BuildRoleEnvironmentExceptionCreator())
                .SingleInstance()
                .As<Func<string, RoleEnvironmentException>>();

            builder.RegisterType<StandaloneAzureSettings>()
                .WithParameter("configurationPath", _configuration.ConfigurationPath)
                .WithParameter("roleName", _configuration.RoleName)
                .SingleInstance()
                .As<IAzureSettings>();

            builder.RegisterType<StandaloneAzureRoleInformation>()
                .WithParameter("roleName", _configuration.RoleName)
                .As<IAzureRoleInformation>();
        }
    }
}