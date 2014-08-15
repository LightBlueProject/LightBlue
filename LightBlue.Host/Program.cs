using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using LightBlue.Infrastructure;

namespace LightBlue.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var hostArgs = HostArgs.ParseArgs(args);

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(hostArgs.Assembly),
            };
            var configurationFile = hostArgs.Assembly + ".config";
            if (File.Exists(configurationFile))
            {
                RemoveAzureTraceListenerFromConfiguration(configurationFile);
                appDomainSetup.ConfigurationFile = configurationFile;
            }

            var appDomain = AppDomain.CreateDomain(
                "LightBlue",
                null,
                appDomainSetup);

            Trace.Listeners.Add(new ConsoleTraceListener());

            var runner = (HostRunner) appDomain.CreateInstanceAndUnwrap(
                typeof(HostRunner).Assembly.FullName,
                typeof(HostRunner).FullName);

            runner.ConfigureTracing(new TraceShipper());
            runner.Run(workerRoleAssembly: hostArgs.Assembly,
                configurationPath: hostArgs.ConfigurationPath,
                serviceDefinitionPath: hostArgs.ServiceDefinitionPath,
                roleName: hostArgs.RoleName);
        }

        private static void RemoveAzureTraceListenerFromConfiguration(string configurationFile)
        {
            var xdocument = XDocument.Load(configurationFile);
            if (xdocument.Root == null)
            {
                return;
            }

            var diagnosticsElement = xdocument.Root.Element("system.diagnostics");
            if (diagnosticsElement == null)
            {
                return;
            }

            var listenersElement = diagnosticsElement.Descendants("listeners")
                .FirstOrDefault(l => l.Parent != null && l.Parent.Name == "trace");
            if (listenersElement == null)
            {
                return;
            }

            foreach (var addElement in listenersElement.Descendants("add")
                .Where(a => a.Attribute("type").Value.StartsWith("Microsoft.WindowsAzure"))
                .ToList())
            {
                addElement.Remove();
            }

            xdocument.Save(configurationFile);
        }
    }
}