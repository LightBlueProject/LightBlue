using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace LightBlue.Tests.Hosts
{
    public class StopWebServerTests : IUseFixture<StoppedWebHostFixture>
    {
        private StoppedWebHostFixture _fixture;

        public void SetFixture(StoppedWebHostFixture data)
        {
            _fixture = data;
        }

        [Fact]
        public void ThenStopWebRole()
        {
            Assert.True(_fixture.Stopped);
        }

        [Fact]
        public async Task ThenStopWebServer()
        {
            await Task.Delay(TimeSpan.FromSeconds(1));

            Exception exception = null;
            try
            {
                var http = new HttpClient();
                http.BaseAddress = new Uri("https://localhost:44399");
                var r = await http.GetAsync("/api/values");
                var content = await r.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                exception = e;
            }
            Assert.NotNull(exception);
        }
    }
}