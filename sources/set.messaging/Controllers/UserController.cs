using System.Threading.Tasks;
using System.Web.Mvc;

using set.messaging.Data.Services;
using set.messaging.Models;

namespace set.messaging.Controllers
{
    public class UserController : BaseController
    {
        private readonly IFormsAuthenticationService _formsAuthenticationService;
        private readonly IUserService _userService;

        public UserController(
            IFormsAuthenticationService formsAuthenticationService,
            IUserService userService)
        {
            _formsAuthenticationService = formsAuthenticationService;
            _userService = userService;
        }

        [HttpGet, AllowAnonymous]
        public ActionResult Login()
        {
            var model = new LoginModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> Login(LoginModel model)
        {
            if (!model.IsValid())
            {
                SetPleaseTryAgain(model);
                return View(model);
            }

            var authenticated = await _userService.Authenticate(model.Email, model.Password);
            if (!authenticated)
            {
                SetPleaseTryAgain(model);
                return View(model);
            }

            var user = await _userService.GetByEmail(model.Email);
            if (user == null)
            {
                SetPleaseTryAgain(model);
                return View(model);
            }

            _formsAuthenticationService.SignIn(user.Id, user.Name, user.Email, user.RoleName, true);

            return Redirect(!string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : "/app/list");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _formsAuthenticationService.SignOut();
            return RedirectToHome();
        }
    }
}