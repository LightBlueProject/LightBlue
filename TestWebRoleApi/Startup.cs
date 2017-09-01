using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using LightBlue.Autofac;
using Owin;

namespace TestWebRoleApi
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        { 
            var http = new HttpConfiguration();
            http.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            appBuilder.UseWebApi(http);

            var builder = new ContainerBuilder();
            builder.RegisterLightBlueModules();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}