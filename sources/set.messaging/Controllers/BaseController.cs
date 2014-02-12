using System.Web.Mvc;

using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.Controllers
{
    public class BaseController : Controller
    {
        public HtmlHelper SetHtmlHelper;

        public BaseController()
        {
            SetHtmlHelper = new HtmlHelper(new ViewContext(), new ViewPage());
        }
        
        public RedirectResult RedirectToHome()
        {
            return Redirect("/");
        }

        public void SetPleaseTryAgain(BaseModel model)
        {
            model.Msg = SetHtmlHelper.LocalizationString("please_check_the_fields_and_try_again");
        }
    }
}