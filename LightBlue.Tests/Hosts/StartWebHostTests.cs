using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using LightBlue.Infrastructure;
using LightBlue.Setup;
using Xunit;

namespace LightBlue.Tests.Hosts
{
    public class StartWebHostTests : IUseFixture<RunningWebHostFixture>
    {
        private RunningWebHostFixture _fixture;

        public void SetFixture(RunningWebHostFixture data)
        {
            _fixture = data;
        }

        [Fact]
        public void ThenWriteLogsTo()
        {
            var log = LightBlueFileSystem.LocalAppData.EnumerateFiles("LightBlue-TestWebRoleApiService*.txt", SearchOption.TopDirectoryOnly)
                .OrderBy(x => x.CreationTime)
                .First();
            Assert.True(log.Exists);
        }

        [Fact]
        public void ThenSetTempDirectoryEnvironmentVariables()
        {
            var tempDirectory = Path.Combine(LightBlueFileSystem.LocalAppData.FullName, "TestWebRoleApi");
            Assert.True(Environment.GetEnvironmentVariable("TMP").StartsWith(tempDirectory));
            Assert.True(Environment.GetEnvironmentVariable("TEMP").StartsWith(tempDirectory));
        }

        [Fact]
        public void ThenCreateLocalAppDataDirectory()
        {
            Assert.True(LightBlueFileSystem.LocalAppData.Exists);
        }

        [Fact]
        public void ThenCreateLightBlueContext()
        {
            Assert.True(LightBlueConfiguration.IsInitialised);
            Assert.True(LightBlueConfiguration.IsLightBlueAppDomainContextType());
        }

        [Fact]
        [UseReporter(typeof(DiffReporter), typeof(TeamCityReporter))]
        public void ThenConfigureWebServer()
        {
            var iisExpress = LightBlueFileSystem.LocalAppData.GetDirectories()
                .Where(x => x.Name.StartsWith("TestWebRoleApiService-iisexpress-"))
                .OrderBy(x => x.CreationTime)
                .First();
            var log = iisExpress.GetFiles("*.log", SearchOption.AllDirectories).Single();
            var config = iisExpress.GetFiles("applicationhost.config").Single();
            var xml = XDocument.Parse(File.ReadAllText(config.FullName));

            Assert.True(log.Exists);
            Approvals.Verify(xml.ToString());
        }

        [Fact]
        public void ThenRunWebRole()
        {
            Assert.True(_fixture.Running);
        }

        [Fact]
        public async Task ThenRunWebServer()
        {
            var http = new HttpClient();
            http.BaseAddress = new Uri("https://localhost:44399");
            var r = await http.GetAsync("/api/values");
            r.EnsureSuccessStatusCode();
            var content = await r.Content.ReadAsStringAsync();

            Assert.Equal("[\"value1\",\"value2\"]", content);
        }
    }
}