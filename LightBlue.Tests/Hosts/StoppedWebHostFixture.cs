using System;
using System.Threading;
using LightBlue.Hosts;
using LightBlue.Setup;

namespace LightBlue.Tests.Hosts
{
    public class StoppedWebHostFixture : IDisposable
    {
        public WebHost.Settings Settings { get; set; }

        public bool Running { get; set; }

        public WebHost Host { get; set; }

        public StoppedWebHostFixture()
        {
            Settings = new WebHost.Settings
            {
                Assembly = @"C:\Source\LightBlue\TestWebRoleApi\bin\TestWebRoleApi.dll",
                Port = "44399",
                RoleName = "TestWebRoleApi",
                ServiceTitle = "TestWebRoleApiService",
                Cscfg = @"C:\Source\LightBlue\TestCloudService\ServiceConfiguration.Local.cscfg",
                Csdef = @"C:\Source\LightBlue\TestCloudService\ServiceDefinition.csdef",
                UseSSL = true,
                Host = "localhost"
            };

            Host = new WebHost(Settings);

            Running = Host.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));

            Stopped = Host.Stop();
        }

        public bool Stopped { get; set; }

        public void Dispose()
        {
            if (Host != null && !Host.IsDisposed)
                Host.Stop();
            LightBlueConfiguration.DestroyLightBlueContext();
        }
    }
}