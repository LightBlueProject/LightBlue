using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Autofac;
using LightBlue.Hosts;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Starter;
using Microsoft.Owin.Logging;
using Topshelf;
using Topshelf.Logging;

namespace LightBlue.OwinHost
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = WebHost.Settings.Load();

            HostFactory.Run(c =>
            {
                Trace.Listeners.Add(new TopshelfConsoleTraceListener());

                c.RunAsNetworkService();

                c.Service<OwinService>(s =>
                {
                    s.ConstructUsing(() => new OwinService(settings));
                    s.WhenStarted((service, control) => service.Start());
                    s.WhenStopped((service, control) => service.Stop());
                });
            });
        }
    }

    internal class OwinService
    {
        private readonly WebHost.Settings _settings;
        private IDisposable _host;
        private IContainer _container;
        private Uri _uri;

        public OwinService(WebHost.Settings settings)
        {
            _settings = settings;
        }

        public bool Start()
        {
            var port = int.Parse(_settings.Port);
            var range = Enumerable.Range(44300, 44399);
            if (range.All(x => x != port))
            {
                throw new InvalidOperationException("Port number outside of SSL range");
            }

            _uri = new Uri(string.Format("https://{0}:{1}/", _settings.Host, port));

            Environment.SetEnvironmentVariable("LightBlueHost", "true");
            Environment.SetEnvironmentVariable("LightBlueConfigurationPath", _settings.Cscfg);
            Environment.SetEnvironmentVariable("LightBlueServiceDefinitionPath", _settings.Csdef);
            Environment.SetEnvironmentVariable("LightBlueRoleName", _settings.RoleName);
            Environment.SetEnvironmentVariable("LightBlueUseHostedStorage", "false");

            Directory.SetCurrentDirectory(_settings.SiteDirectory);

            var options = new StartOptions
            {
                Port = port
            };

            options.Urls.Add(_uri.AbsoluteUri);

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

    public class ServiceProvider : IServiceProvider
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
