using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LightBlue.Setup;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Infrastructure
{
    public class HostRunner
    {
        private RoleEntryPoint _workerRole;

        public void Run(
            string workerRoleAssembly,
            string configurationPath,
            string roleName,
            bool useHostedStorage)
        {
            LightBlueConfiguration.SetAsLightBlue(
                configurationPath: configurationPath,
                roleName: roleName,
                lightBlueHostType: LightBlueHostType.Direct,
                useHostedStorage: useHostedStorage);

            if (workerRoleAssembly.EndsWith(".exe"))
                RunConsoleApplication(workerRoleAssembly);
            else
                RunAzureRole(workerRoleAssembly);
        }

        private static void RunConsoleApplication(string workerRoleAssembly)
        {
            var assembly = Assembly.LoadFrom(workerRoleAssembly);
            assembly.EntryPoint.Invoke(null, new object[] {null});
        }

        private void RunAzureRole(string workerRoleAssembly)
        {
            var workerRoleType = LoadWorkerRoleType(workerRoleAssembly);
            _workerRole = (RoleEntryPoint)Activator.CreateInstance(workerRoleType);
            if (!_workerRole.OnStart())
            {
                Trace.TraceError("Role failed to start for '{0}'", workerRoleType);
                return;
            }
            try
            {
                if (workerRoleType.Name.Contains("WebRole"))
                {
                    Trace.TraceInformation("HostRunner: Will not call Run() on WebRole, waiting for thread control.");
                    try
                    {
                        Task.Delay(-1, LightBlueThreadControl.CancellationToken).Wait();
                    }
                    catch (AggregateException)
                    {
                        Trace.TraceError("HostRunner: Cancellation requested, terminating.");
                    }
                    catch (OperationCanceledException)
                    {
                        Trace.TraceError("HostRunner: Cancellation requested, terminating.");
                    }
                }
                else
                {
                    _workerRole.Run();
                }
            }
            finally
            {
                _workerRole.OnStop();
            }
        }

        private static Type LoadWorkerRoleType(string workerRoleAssembly)
        {
            var roleAssemblyAbsolutePath = Path.IsPathRooted(workerRoleAssembly)
                ? workerRoleAssembly
                : Path.Combine(Environment.CurrentDirectory, workerRoleAssembly);

            var roleAssembly = Assembly.LoadFrom(roleAssemblyAbsolutePath);
            var workerRoleType = roleAssembly.GetTypes()
                .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
            return workerRoleType;
        }
    }
}