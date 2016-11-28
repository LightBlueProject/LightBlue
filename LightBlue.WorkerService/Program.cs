using System.Diagnostics;
using LightBlue.Hosts;
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

                var settings = WorkerHost.Settings.Load();

                x.Service<WorkerHost>(service =>
                {
                    service.ConstructUsing(s => new WorkerHost(settings));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(string.Format("LightBlue {0} WorkerRole Windows Service", settings.ServiceTitle));
                x.SetDisplayName(settings.ServiceTitle + " Service");
                x.SetServiceName(settings.ServiceTitle);
                x.StartManually();
            });
        }
    }
}
