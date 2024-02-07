using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Autofac;

using LightBlue;
using LightBlue.Autofac;
using LightBlue.WorkerRoleDependency;

namespace TestWorkerRole
{
    public class WorkerRole 
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        private static void Main(string[] args)
        {
            var worker = new WorkerRole();
            worker.Run();
        }

        public void Run()
        {
            Trace.TraceInformation("TestWorkerRole has been started");
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterLightBlueModules();
            _container = containerBuilder.Build();

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