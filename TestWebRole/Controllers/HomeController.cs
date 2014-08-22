using System.IO;
using System.Web.Mvc;

using LightBlue;

namespace TestWebRole.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAzureEnvironmentSource _azureEnvironmentSource;

        public HomeController(IAzureEnvironmentSource azureEnvironmentSource)
        {
            _azureEnvironmentSource = azureEnvironmentSource;
        }

        public ActionResult Index()
        {
            ViewBag.Message = _azureEnvironmentSource.CurrentEnvironment + " " + Path.GetTempPath();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}