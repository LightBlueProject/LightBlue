using System;
using System.Diagnostics;
using LightBlue.Host.Stub;

namespace LightBlue.Host
{
    internal static class WorkerHostFactory
    {
        public static HostStub Create(HostAssemblyCommand.Settings settings)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = settings.ApplicationBase
            };

            appDomainSetup.ConfigurationFile = settings.RoleConfigurationFile;

            StubManagement.CopyStubAssemblyToRoleDirectory(settings.ApplicationBase);

            var appDomain = AppDomain.CreateDomain(
                "LightBlue",
                null,
                appDomainSetup);

            Trace.Listeners.Add(new ConsoleTraceListener());

            var stub = (HostStub) appDomain.CreateInstanceAndUnwrap(
                typeof(HostStub).Assembly.FullName,
                typeof(HostStub).FullName);

            stub.ConfigureTracing(new ConsoleTraceShipper());

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionBehaviour.UnhandledExceptionHandler(settings.WindowTitle);

            return stub;
        }
    }
}