using System;
using System.Net.Http;
using System.Threading.Tasks;
using LightBlue.Hosts;
using LightBlue.OwinHost;
using Xunit;

namespace LightBlue.Tests.Owin
{
    public class OwinHostTests : IDisposable
    {
        private readonly OwinService _host;
        private readonly WebHost.Settings _settings;

        public OwinHostTests()
        {
            _settings = WebHost.Settings.Load();
            _host = new OwinService(_settings);
        }

        [Fact]
        public async Task CanRunServer()
        {
            var running = _host.Start();

            using (var http = new HttpClient())
            {
                http.BaseAddress = new Uri("https://" + _settings.Host + ":" + _settings.Port);
                var response = await http.GetAsync("/api/values");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                Assert.True(running);
                Assert.Equal("[\"value1\",\"value2\"]", content);
            }
        }

        public void Dispose()
        {
            _host.Stop();
        }
    }
}