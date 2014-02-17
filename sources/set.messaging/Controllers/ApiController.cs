using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using set.messaging.Data.Services;
using set.messaging.Helpers;

namespace set.messaging.Controllers
{
    public class ApiController : BaseController
    {
        private readonly IAppService _appService;
        private readonly IMessageService _messageService;

        public ApiController(
            IAppService appService,
            IMessageService messageService)
        {
            _appService = appService;
            _messageService = messageService;
        }

        [HttpPost, ValidateInput(false)]
        public async Task<JsonResult> SendEmail(string to, string subject, string htmlBody)
        {
            if (!to.IsEmail()
                || string.IsNullOrWhiteSpace(subject)
                || string.IsNullOrWhiteSpace(htmlBody))
            {
                throw new HttpException(400, "not valid");
            }

            var response = await _messageService.SendEmail(to, subject, htmlBody);

            return Json(new { response.HttpStatusCode, response.MessageId, response.ContentLength }, JsonRequestBehavior.DenyGet);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var headers = filterContext.RequestContext.HttpContext.Request.Headers;

            var authHeader = headers[ConstHelper.Authorization];
            if (authHeader == null) ReturnNotAuthenticated(filterContext);

            var token = authHeader;
            if (string.IsNullOrWhiteSpace(token)) ReturnNotAuthenticated(filterContext);

            var isTokenValidTask = _appService.IsTokenValid(token);
            isTokenValidTask.Wait();

            if (!isTokenValidTask.Result) ReturnNotAuthenticated(filterContext);

            try
            {
                _appService.LogRequest(token, Request.UserHostAddress, Request.Url.AbsolutePath);
            }
            catch { }

            base.OnActionExecuting(filterContext);
        }

        private static void ReturnNotAuthenticated(ControllerContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.Clear();
            filterContext.RequestContext.HttpContext.Response.Write("your request is not authenticated with valid token!");
            filterContext.RequestContext.HttpContext.Response.End();
        }
    }
}