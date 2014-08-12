using System;
using System.Reflection;

using Autofac;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Standalone
{
    public class LightBlueStandaloneModule : Autofac.Module
    {
        private readonly StandaloneConfiguration _configuration;
        private readonly Func<string, RoleEnvironmentException> _roleEnvironmentExceptionCreator;

        public LightBlueStandaloneModule(StandaloneConfiguration configuration, Func<string, RoleEnvironmentException> roleEnvironmentExceptionCreator)
        {
            _configuration = configuration;
            _roleEnvironmentExceptionCreator = roleEnvironmentExceptionCreator;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .InNamespace(typeof(LightBlueStandaloneModule).Namespace)
                .AsImplementedInterfaces();

            builder.RegisterType<StandaloneAzureEnvironmentSource>()
                .SingleInstance()
                .As<IAzureEnvironmentSource>();

            builder.RegisterInstance(_roleEnvironmentExceptionCreator)
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

            builder.RegisterInstance((Func<string, IAzureStorage>)(connectionString => new StandaloneAzureStorage(connectionString)))
                .As<Func<string, IAzureStorage>>();

            builder.RegisterInstance((Func<Uri, IAzureBlockBlob>)(blobUri => new StandaloneAzureBlockBlob(blobUri)))
                .As<Func<Uri, IAzureBlockBlob>>();
        }
    }
}