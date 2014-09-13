using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using Castle.Windsor;

using LightBlue;
using LightBlue.Windsor;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace TestWindsorRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IWindsorContainer _container;

        public override void Run()
        {
            Trace.TraceInformation("TestWindorRole is running");

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
            ServicePointManager.DefaultConnectionLimit = 12;

            var result = base.OnStart();

            _container = new WindsorContainer();
            _container.InstallLightBlue();

            Trace.TraceInformation("TestWindorRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("TestWindorRole is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("TestWindorRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation(
                    "Working: "
                    + _container.Resolve<IAzureSettings>()["WindsorSetting"]);

                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}