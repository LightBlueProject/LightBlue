using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

using LightBlue.Setup;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Infrastructure
{
    public class HostRunner
    {
        private RoleEntryPoint _workerRole;

        public void Run(string workerRoleAssembly, string configurationPath, string serviceDefinitionPath, string roleName)
        {
            LightBlueConfiguration.SetAsLightBlue(
                configurationPath: configurationPath,
                serviceDefinitionPath: serviceDefinitionPath,
                roleName: roleName,
                lightBlueHostType: LightBlueHostType.Direct);

            try
            {
                var workerRoleType = LoadWorkerRoleType(workerRoleAssembly);

                var thread = new Thread(() => RunRole(workerRoleType));
                thread.Start();

                thread.Join();
                _workerRole.OnStop();
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToTraceMessage());
            }
        }

        private void RunRole(Type workerRoleType)
        {
            _workerRole = (RoleEntryPoint)Activator.CreateInstance(workerRoleType);
            if (!_workerRole.OnStart())
            {
                Trace.TraceError("Role failed to start for '{0}'", workerRoleType);
                return;
            }

            _workerRole.Run();
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