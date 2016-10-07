using LightBlue.MultiHost.Core.IISExpress;
using LightBlue.MultiHost.Core.Runners;
using LightBlue.Setup;

namespace LightBlue.MultiHost.Core.Configuration
{
    public class MultiHostRoot
    {
        public static MultiHostConfiguration Configuration { get; private set; }

        public static void Configure(MultiHostConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static void Start()
        {
            var assemblyLocations = Configuration.GetAssemblyLocations();
            ThreadRunnerAssemblyCache.Initialise(assemblyLocations);
            IisExpressHelper.KillIisExpressProcesses();
            LightBlueConfiguration.SetAsMultiHost();
        }
    }
}