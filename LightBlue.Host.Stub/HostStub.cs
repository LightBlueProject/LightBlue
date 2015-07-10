using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace LightBlue.Host.Stub
{
    public class HostStub : MarshalByRefObject
    {
        private readonly object gate = new object();
        private CancellationTokenSource _cancellationTokenSource;

        // We don't want the remoting connection between parent -> child app domain timing out.
        // http://stackoverflow.com/questions/2410221/appdomain-and-marshalbyrefobject-life-time-how-to-avoid-remotingexception
        public override object InitializeLifetimeService()
        {
            return null;
        }

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
            
            lock (gate)
            {
                // as hoststub is called asynchronously, it it possible that shutdown has already been requested
                // by the time we reach this point.
                if (_cancellationTokenSource != null && _cancellationTokenSource.IsCancellationRequested) return;

                var threadControlType = lightBlueAssembly.GetType("LightBlue.LightBlueThreadControl");
                var getMethod = threadControlType.GetMethod("get_CancellationTokenSource", BindingFlags.NonPublic | BindingFlags.Static);
                _cancellationTokenSource = (CancellationTokenSource)getMethod.Invoke(null, null);
            }

            var runner = Activator.CreateInstance(runnerType);
            runMethod.Invoke(runner, new object[] { workerRoleAssembly, configurationPath, serviceDefinitionPath, roleName, useHostedStorage });
        }
        
        public void RequestShutdown()
        {
            lock (gate)
            {
                if (_cancellationTokenSource == null)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _cancellationTokenSource.Cancel();
                }
                else
                {
                    _cancellationTokenSource.Cancel();
                }
            }
        }
    }

    public static class StubExceptionHandler
    {
        public static UnhandledExceptionEventHandler Handler = (sender, args) => { };
    }
}