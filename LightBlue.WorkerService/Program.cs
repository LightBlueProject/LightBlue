using System.Diagnostics;
using Topshelf;
using Topshelf.Logging;

namespace LightBlue.WorkerService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                Trace.Listeners.Add(new TopshelfConsoleTraceListener());

                var settings = Settings.Load();

                x.Service<WorkerHost>(service =>
                {
                    service.ConstructUsing(s => new WorkerHost(settings));
                    service.WhenStarted((s, hc) => s.Start(hc));
                    service.WhenStopped(s => { });
                });

                x.RunAsLocalSystem();
                x.SetDescription(string.Format("LightBlue {0} WorkerRole Windows Service", settings.ServiceTitle));
                x.SetDisplayName(settings.ServiceTitle + " Service");
                x.SetServiceName(settings.ServiceTitle);
                x.StartAutomatically();
                x.EnableShutdown();
                x.EnableServiceRecovery(rc =>
                {
                    rc.RestartService(0); // first failure
                    rc.RestartService(0); // second failure
                    rc.RestartService(0); // subsequent failures
                });
            });
        }
    }
}
