using System.Reflection;
using System.Runtime;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Autofac;
using Autofac.Integration.Mvc;

using LightBlue.Setup;

namespace TestWebRole
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            ConfigureAutofac();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void ConfigureAutofac()
        {
            var builder = new ContainerBuilder();
            builder.RegisterLightBlueWebModules();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}