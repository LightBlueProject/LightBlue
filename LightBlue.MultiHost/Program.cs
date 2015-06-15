using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Host;
using LightBlue.Infrastructure;
using LightBlue.Setup;

namespace LightBlue.MultiHost
{
    public class NewRelicFilteringTraceListener : TraceListener
    {
        public override void Write(string message)
        {
            if (message.Contains("NewRelic")) return;
            Console.Write(message);
        }

        public override void WriteLine(string message)
        {
            if (message.Contains("NewRelic")) return;
            Console.WriteLine(message);
        }
    }

    public static class Program
    {
        private static readonly ConcurrentBag<string> AllRoleFolders = new ConcurrentBag<string>();
        static int _roleCount;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += TryResolve;

            Console.Title = "Multi-Host LightBlue emulator";

            var basePath = args[0];
            var configFile = args[1].Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Trace.Listeners.Add(new NewRelicFilteringTraceListener());
            AppDomain.CurrentDomain.UnhandledException +=
                UnhandledExceptionBehaviour.UnhandledExceptionHandler("Multi-host");

            foreach (var configLine in configFile)
            {
                var replacedConfigLine = configLine.Replace("$parentDirectory", basePath);
                var hostArgs = HostArgs.ParseArgs(replacedConfigLine.Split(' '));
                if (hostArgs == null)
                {
                    return;
                }

                ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(hostArgs.RoleConfigurationFile);

                Console.Title = "Multi-Host - hosting " + (++_roleCount) + " roles";

                var hostRunner = new HostRunner();

                var roleDirectory = Path.GetDirectoryName(hostArgs.Assembly);
                BootstrapLightBlueAssembly();
                AllRoleFolders.Add(roleDirectory);

                Task.Factory.StartNew(
                () =>
                    hostRunner.Run(hostArgs.Assembly, hostArgs.ConfigurationPath, hostArgs.ServiceDefinitionPath, hostArgs.RoleName, hostArgs.UseHostedStorage)
                    , TaskCreationOptions.LongRunning).ContinueWith(t =>
                    {
                        if (!hostArgs.AllowSilentFail)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "The host {0} has exited unexpectedly",
                                    hostArgs.Title));
                        }
                    });
            }

            Console.ReadLine(); 
        }

        private static void BootstrapLightBlueAssembly()
        {
            LightBlueConfiguration.RunAsMultiHost();
        }

        private static Assembly TryResolve(object sender, ResolveEventArgs args)
        {
            foreach (var allLogFolder in AllRoleFolders)
            {
                if (File.Exists(Path.Combine(allLogFolder, new AssemblyName(args.Name).Name + ".dll")))
                    return Assembly.LoadFrom(Path.Combine(allLogFolder, new AssemblyName(args.Name).Name + ".dll"));
            }
            return null;
        }
    }
}