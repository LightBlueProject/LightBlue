using System.Diagnostics;
using LightBlue.Hosts;
using Topshelf;
using Topshelf.Logging;

namespace LightBlue.WebService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int) HostFactory.Run(x =>
            {
                Trace.Listeners.Add(new TopshelfConsoleTraceListener());

                var settings = WebHost.Settings.Load();

                x.Service<WebHost>(service =>
                {
                    service.ConstructUsing(s => new WebHost(settings));
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(string.Format("LightBlue {0} WebRole Windows Service", settings.ServiceTitle));
                x.SetDisplayName(settings.ServiceTitle + " Service");
                x.SetServiceName(settings.ServiceTitle);
                x.StartManually();
            });
        }
    }
}
