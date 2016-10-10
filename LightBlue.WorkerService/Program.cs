using Formo;
using LightBlue.Host;
using Topshelf;

namespace LightBlue.WorkerService
{
    class Program
    {
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                dynamic configuration = new Configuration();
                WorkerHostSettings settings = configuration.Bind<WorkerHostSettings>();

                x.Service<WorkerHostService>(service =>
                {
                    service.ConstructUsing(s =>
                    {
                        var hostArgs = HostArgs.ParseArgs(new[]
                        {
                            "-a:" + settings.Assembly,
                            "-n:" + settings.RoleName,
                            "-t:" + settings.ServiceTitle,
                            "-c:" + settings.Configuration
                        });
                        return new WorkerHostService(hostArgs);
                    });

                    service.WhenStarted((s, h) => s.Start(h));
                    service.WhenStopped((s, h) => s.Stop(h));
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
