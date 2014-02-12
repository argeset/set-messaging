using System.Web.Mvc;

namespace set.messaging.Controllers
{
    [AllowAnonymous]
    public class HomeController  : BaseController
    {
        public ViewResult Index()
        {
            return View();
        }
    }
}