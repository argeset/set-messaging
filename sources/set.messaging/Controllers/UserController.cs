using System.Threading.Tasks;
using System.Web.Mvc;

using set.messaging.Data.Services;
using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.Controllers
{
    public class UserController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public UserController(
            IAuthService authService,
            IUserService userService)
        {
            _authService = authService;
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

            _authService.SignIn(user.Id, user.Name, user.Email, user.RoleName, true);

            return Redirect(!string.IsNullOrEmpty(model.ReturnUrl) ? model.ReturnUrl : "/app/list");
        }


        [HttpGet, AllowAnonymous]
        public ActionResult PasswordReset()
        {
            var model = new PasswordResetModel();

            if (User.Identity.IsAuthenticated)
            {
                model.Email = User.Identity.GetEmail();
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken, AllowAnonymous]
        public async Task<ActionResult> PasswordReset(PasswordResetModel model)
        {
            SetPleaseTryAgain(model);
            if (model.IsNotValid())
            {
                return View(model);
            }

            var isOk = await _userService.RequestPasswordReset(model.Email);
            if (isOk)
            {
                model.Msg = "password_reset_request_successful".Localize();
            }

            return View(model);
        }

        [HttpGet, AllowAnonymous]
        public async Task<ActionResult> PasswordChange(string email, string token)
        {
            var model = new PasswordChangeModel { Email = email, Token = token };

            if (!await _userService.IsPasswordResetRequestValid(model.Email, model.Token))
            {
                return Redirect("/User/Login");
            }

            return View(model);
        }

        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> PasswordChange(PasswordChangeModel model)
        {
            SetPleaseTryAgain(model);
            if (model.IsNotValid())
            {
                return View(model);
            }

            if (!await _userService.ChangePassword(model.Email, model.Token, model.Password))
            {
                return View(model);
            }

            return Redirect("/User/Login");
        }

        [HttpGet]
        public ActionResult Logout()
        {
            _authService.SignOut();
            return RedirectToHome();
        }
    }
}