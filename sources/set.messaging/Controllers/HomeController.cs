using System.Web.Mvc;

namespace set.messaging.Controllers
{
    [AllowAnonymous]
    public class HomeController  : Controller
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}