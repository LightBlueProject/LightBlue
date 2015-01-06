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
    }
}