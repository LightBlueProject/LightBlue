using System.Diagnostics;
using System.Threading.Tasks;

using Autofac;

using LightBlue;
using LightBlue.Autofac;

using Microsoft.WindowsAzure.ServiceRuntime;

namespace TestWebRole
{
    public class WebRole : RoleEntryPoint
    {
        private IContainer _container;

        public override bool OnStart()
        {
            var result = base.OnStart();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterLightBlueModules();

            _container = containerBuilder.Build();

            Trace.TraceInformation("TestWebRole has been started");

            return result;
        }

        public override void Run()
        {
            while (true)
            {
                Trace.TraceInformation("Working: " + _container.Resolve<IAzureEnvironmentSource>().CurrentEnvironment);

                Task.Delay(1000).Wait();
            }
        }
    }
}