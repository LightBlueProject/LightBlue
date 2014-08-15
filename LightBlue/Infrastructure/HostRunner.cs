using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using LightBlue.Setup;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Infrastructure
{
    public class HostRunner : MarshalByRefObject
    {
        public void ConfigureTracing(TraceShipper traceShipper)
        {
            Trace.Listeners.Add(new CrossDomainTraceListener(traceShipper));
        }

        public void Run(string workerRoleAssembly, string configurationPath, string serviceDefinitionPath, string roleName)
        {
            SetupConfiguration.SetAsHosted(
                configurationPath: configurationPath,
                serviceDefinitionPath: serviceDefinitionPath,
                roleName: roleName);

            try
            {
                var workerRoleType = LoadWorkerRoleType(workerRoleAssembly);

                var workerRole = (RoleEntryPoint) Activator.CreateInstance(workerRoleType);
                if (!workerRole.OnStart())
                {
                    Trace.TraceError("Role failed to start for '{0}'", workerRoleType);
                }

                try
                {
                    workerRole.Run();
                }
                finally
                {
                    workerRole.OnStop();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToTraceMessage());
            }
        }

        private static Type LoadWorkerRoleType(string workerRoleAssembly)
        {
            var roleAssemblyAbsolutePath = Path.IsPathRooted(workerRoleAssembly)
                ? workerRoleAssembly
                : Path.Combine(Environment.CurrentDirectory, workerRoleAssembly);

            var roleAssembly = Assembly.LoadFile(roleAssemblyAbsolutePath);
            var workerRoleType = roleAssembly.GetTypes()
                .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
            return workerRoleType;
        }
    }
}