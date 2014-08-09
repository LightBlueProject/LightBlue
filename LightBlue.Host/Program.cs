using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

using LightBlue.Infrastructure;
using LightBlue.Standalone;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace LightBlue.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var hostArgs = HostArgs.ParseArgs(args);

            ModuleSource.ClearModules();
            ModuleSource.AddModule(new LightBlueStandaloneModule(new StandaloneConfiguration
            {
                ConfigurationPath = hostArgs.ConfigurationPath,
                RoleName = hostArgs.RoleName
            }));


            var workerRoleType = LoadWorkerRoleType(hostArgs);

            var runState = RunState.NotRun;
            while (runState.ShouldRunHost(hostArgs))
            {
                runState = RunWorkerRole(workerRoleType);
            }
        }

        private static RunState RunWorkerRole(Type workerRoleType)
        {
            try
            {
                var workerRole = (RoleEntryPoint) Activator.CreateInstance(workerRoleType);
                if (!workerRole.OnStart())
                {
                    Trace.TraceError("Role failed to start for '{0}'", workerRoleType);
                    return RunState.FailedToStart;
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
                return RunState.ThrewException;
            }
            return RunState.ExitedCleanly;
        }

        private static Type LoadWorkerRoleType(HostArgs hostArgs)
        {
            var roleAssemblyAbsolutePath = Path.IsPathRooted(hostArgs.Assembly)
                ? hostArgs.Assembly
                : Path.Combine(Environment.CurrentDirectory, hostArgs.Assembly);

            var roleAssembly = Assembly.LoadFile(roleAssemblyAbsolutePath);
            var workerRoleType = roleAssembly.GetTypes()
                .Single(t => typeof(RoleEntryPoint).IsAssignableFrom(t));
            return workerRoleType;
        }
    }
}