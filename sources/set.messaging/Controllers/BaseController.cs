using System.Web.Mvc;

using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.Controllers
{
    public class BaseController : Controller
    {
        public RedirectResult RedirectToHome()
        {
            return Redirect("/");
        }

        public void SetPleaseTryAgain(BaseModel model)
        {
            model.Msg = "please_check_the_fields_and_try_again".Localize();
        }
    }
}