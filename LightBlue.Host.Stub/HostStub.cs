using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace LightBlue.Host.Stub
{
    public class HostStub : MarshalByRefObject
    {
        public void ConfigureTracing(TraceShipper traceShipper)
        {
            Trace.Listeners.Add(new CrossDomainTraceListener(traceShipper));
        }

        public void Run(
            string workerRoleAssembly,
            string configurationPath,
            string serviceDefinitionPath,
            string roleName,
            bool useHostedStorage)
        {
            var roleDirectory = Path.GetDirectoryName(workerRoleAssembly);

            if (string.IsNullOrWhiteSpace(roleDirectory))
            {
                throw new ArgumentException("The worker role assembly must be in a specified directory.");
            }
            Directory.SetCurrentDirectory(roleDirectory);

            var lightBlueAssembly = Assembly.LoadFrom(Path.Combine(roleDirectory, "LightBlue.dll"));
            var runnerType = lightBlueAssembly.GetType("LightBlue.Infrastructure.HostRunner");
            var runMethod = runnerType.GetMethod("Run");
            var runner = Activator.CreateInstance(runnerType);
            runMethod.Invoke(runner, new object[] {workerRoleAssembly, configurationPath, serviceDefinitionPath, roleName, useHostedStorage});
        }
    }
}