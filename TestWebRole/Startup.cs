using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestWebRole.Startup))]
namespace TestWebRole
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
