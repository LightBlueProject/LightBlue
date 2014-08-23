using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using LightBlue;
using LightBlue.Autofac;
using LightBlue.WorkerRoleDependency;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace TestWorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.TraceInformation("TestWorkerRole is running");

            try
            {
                RunAsync(_cancellationTokenSource.Token).Wait();
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnStart();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterLightBlueModules();

            _container = containerBuilder.Build();

            Trace.TraceInformation("TestWorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TestWorkerRole is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TestWorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var randomDependency = new RandomDependency();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation(
                    "Working: "
                    + _container.Resolve<IAzureSettings>()["RandomSetting"]
                    + " "
                    + randomDependency.RandomNumber()
                    + " "
                    + Path.GetTempPath());
                
                await Task.Delay(1000);
            }
        }
    }
}