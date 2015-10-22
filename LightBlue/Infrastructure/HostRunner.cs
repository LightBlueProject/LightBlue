using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
            string serviceDefinitionPath,
            string roleName,
            bool useHostedStorage)
        {
            LightBlueConfiguration.SetAsLightBlue(
                configurationPath: configurationPath,
                serviceDefinitionPath: serviceDefinitionPath,
                roleName: roleName,
                lightBlueHostType: LightBlueHostType.Direct,
                useHostedStorage: useHostedStorage);

            var workerRoleType = LoadWorkerRoleType(workerRoleAssembly);
            RunRole(workerRoleType);
        }

        private void RunRole(Type workerRoleType)
        {
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