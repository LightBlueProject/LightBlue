using System;
using System.Diagnostics;
using LightBlue.Host.Stub;

namespace LightBlue.Host
{
    public static class WorkerHostFactory
    {
        public static HostStub Create(HostArgs hostArgs)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = hostArgs.ApplicationBase
            };

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

            stub.ConfigureTracing(new ConsoleTraceShipper());

            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionBehaviour.UnhandledExceptionHandler(hostArgs.Title);

            return stub;
        }
    }
}