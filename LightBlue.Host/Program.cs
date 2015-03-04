using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

using LightBlue.Host.Stub;
using LightBlue.Infrastructure;

namespace LightBlue.Host
{
    public static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hWnd, string text);

        public static void Main(string[] args)
        {
            var hostArgs = HostArgs.ParseArgs(args);
            if (hostArgs == null)
            {
                return;
            }

            var handle = Process.GetCurrentProcess().MainWindowHandle;

            SetWindowText(handle, hostArgs.Title);

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = hostArgs.ApplicationBase
            };

            ConfigurationManipulation.RemoveAzureTraceListenerFromConfiguration(hostArgs.RoleConfigurationFile);
            appDomainSetup.ConfigurationFile = hostArgs.RoleConfigurationFile;

            StubManagement.CopyStubAssemblyToRoleDirectory(hostArgs.ApplicationBase);

            var appDomain = AppDomain.CreateDomain(
                "LightBlue",
                null,
                appDomainSetup);

            Trace.Listeners.Add(new ConsoleTraceListener());

            var stub = (HostStub) appDomain.CreateInstanceAndUnwrap(
                typeof(HostStub).Assembly.FullName,
                typeof(HostStub).FullName);

            stub.ConfigureTracing(new TraceShipper());

            AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
            {
                var originalColours = SetEmphasisConsoleColours();

                Console.WriteLine(
                    "The hosted application {0} has thrown an unhandled exception",
                    hostArgs.Title);
                Console.WriteLine();
                Console.WriteLine("Options:");
                Console.WriteLine("Press 'x' to kill the process without viewing the error");
                Console.WriteLine("Press 'd' to launch the debugger");
                Console.WriteLine("Press 't' to throw the exception");
                Console.WriteLine("Press anything else to write the exception to the console and exit");

                var option = Console.ReadKey();
                Console.WriteLine();

                switch (option.KeyChar)
                {
                    case 'x':
                    case 'X':
                        RestoreConsoleColours(originalColours);
                        Environment.Exit(1);
                        return;
                    case 'd':
                    case 'D':
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        else
                        {
                            Debugger.Launch();
                        }
                        break;
                    case 't':
                    case 'T':
                        RestoreConsoleColours(originalColours);
                        return;
                    default:
                        var exception = eventArgs.ExceptionObject as Exception;
                        if (exception == null)
                        {
                            Console.WriteLine("Unhandled exception cannot be cast to System.Exception");
                            if (eventArgs.ExceptionObject != null)
                            {
                                Console.WriteLine(eventArgs.ExceptionObject.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine(exception.ToTraceMessage());
                        }
                        RestoreConsoleColours(originalColours);
                        Environment.Exit(1);
                        break;
                }
                RestoreConsoleColours(originalColours);
            };

            stub.Run(workerRoleAssembly: hostArgs.Assembly,
                configurationPath: hostArgs.ConfigurationPath,
                serviceDefinitionPath: hostArgs.ServiceDefinitionPath,
                roleName: hostArgs.RoleName,
                useHostedStorage: hostArgs.UseHostedStorage);

            if (!hostArgs.AllowSilentFail)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "The host {0} has exited unexpectedly",
                        hostArgs.Title));
            }
        }

        private static void RestoreConsoleColours(ConsoleColor[] originalColours)
        {
            Console.ForegroundColor = originalColours[0];
            Console.BackgroundColor = originalColours[1];
        }

        private static ConsoleColor[] SetEmphasisConsoleColours()
        {
            var originalColours = new[] {Console.ForegroundColor, Console.BackgroundColor};
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Red;
            return originalColours;
        }
    }
}