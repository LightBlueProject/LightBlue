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

                c.Service<OwinService>(x =>
                {
                    x.ConstructUsing(() => new OwinService(settings));
                    x.WhenStarted((sc, hc) => sc.Start());
                    x.WhenStopped((sc, hc) => sc.Stop());
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
        private readonly Uri _uri;
        private IDisposable _server;
        private IContainer _container;

        public OwinService(WebHost.Settings settings)
        {
            _settings = settings;
            _uri = new Uri(string.Format("https://{0}:{1}/", _settings.Host, _settings.Port));
        }

        public bool Start()
        {
            Netsh.AddUrlAcl(_uri);

            Environment.SetEnvironmentVariable("LightBlueHost", "true");
            Environment.SetEnvironmentVariable("LightBlueConfigurationPath", _settings.Cscfg);
            Environment.SetEnvironmentVariable("LightBlueServiceDefinitionPath", _settings.Csdef);
            Environment.SetEnvironmentVariable("LightBlueRoleName", _settings.RoleName);
            Environment.SetEnvironmentVariable("LightBlueUseHostedStorage", "false");

            Directory.SetCurrentDirectory(_settings.SiteDirectory);

            _container = CompositionRoot.CreateContainer();

            var starterFactory = _container.Resolve<IHostingStarterFactory>();
            var starter = starterFactory.Create("Domain");
            var options = new StartOptions
            {
                Port = int.Parse(_settings.Port)
            };
            options.Urls.Add(_uri.AbsoluteUri);
            _server = starter.Start(options);

            return true;
        }

        public bool Stop()
        {
            Netsh.DeleteUrlAcl(_uri);

            _server.Dispose();
            _container.Dispose();

            return true;
        }
    }

    internal class Netsh
    {
        public static void AddUrlAcl(Uri uri)
        {
            var netsh = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\netsh.exe");
            var process = Process.Start(netsh, string.Format("http add urlacl url={0} user=\"NT AUTHORITY\\NetworkService\"", uri.OriginalString));
            process.WaitForExit();
            Trace.TraceInformation("Deleted Url ACL for Network Service {0} with exit code {1}", uri.AbsoluteUri, process.ExitCode);
        }

        public static void DeleteUrlAcl(Uri uri)
        {
            var netsh = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32\\netsh.exe");
            var process = Process.Start(netsh, string.Format("http delete urlacl url={0}", uri.AbsoluteUri));
            process.WaitForExit();
            Trace.TraceInformation("Deleted Url ACL for Network Service {0} with exit code {1}", uri.AbsoluteUri, process.ExitCode);
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
