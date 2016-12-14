using System;
using System.Diagnostics;
using System.IO;
using Autofac;
using LightBlue.Hosts;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Starter;
using Topshelf;
using Topshelf.Logging;

namespace LightBlue.OwinHost
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(c =>
            {
                Trace.Listeners.Add(new TopshelfConsoleTraceListener());

                var settings = WebHost.Settings.Load();

                c.Service<OwinService>(s =>
                {
                    s.ConstructUsing(() => new OwinService(settings));
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });

                c.RunAsNetworkService();
                c.SetDescription(string.Format("LightBlue {0} Owin Host Windows Service", settings.ServiceTitle));
                c.SetDisplayName(settings.ServiceTitle + " Service");
                c.SetServiceName(settings.ServiceTitle);
                c.StartManually();
            });
        }
    }

    internal class OwinService
    {
        private readonly WebHost.Settings _settings;
        private IDisposable _host;
        private IContainer _container;

        public OwinService(WebHost.Settings settings)
        {
            _settings = settings;
        }

        public bool Start()
        {
            Environment.SetEnvironmentVariable("LightBlueHost", "true");
            Environment.SetEnvironmentVariable("LightBlueConfigurationPath", _settings.Cscfg);
            Environment.SetEnvironmentVariable("LightBlueServiceDefinitionPath", _settings.Csdef);
            Environment.SetEnvironmentVariable("LightBlueRoleName", _settings.RoleName);
            Environment.SetEnvironmentVariable("LightBlueUseHostedStorage", "false");

            Directory.SetCurrentDirectory(_settings.SiteDirectory);

            var options = new StartOptions
            {
                Port = int.Parse(_settings.Port)
            };

            options.Urls.Add(new Uri(string.Format("https://{0}:{1}/", _settings.Host, options.Port)).AbsoluteUri);

            _container = CompositionRoot.CreateContainer();

            var starterFactory = _container.Resolve<IHostingStarterFactory>();
            var starter = starterFactory.Create("Domain");

            _host = starter.Start(options);

            return true;
        }

        public bool Stop()
        {
            _host.Dispose();
            _container.Dispose();
            return true;
        }
    }

    internal class CompositionRoot
    {
        public static IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>();
            builder.RegisterType<DomainHostingStarter>().AsSelf();
            ServicesFactory.ForEach((service, implementation) => builder.RegisterType(implementation).As(service));
            return builder.Build();
        }
    }

    internal class ServiceProvider : IServiceProvider
    {
        private readonly IComponentContext _componentContext;

        public ServiceProvider(IComponentContext componentContext)
        {
            _componentContext = componentContext;
        }

        public object GetService(Type serviceType)
        {
            return _componentContext.Resolve(serviceType);
        }
    }
}
