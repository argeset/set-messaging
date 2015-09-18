using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

using set.messaging.Data.Services;
using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.Controllers
{
    public class AppController : BaseController
    {
        private readonly IAppService _appService;
        public AppController(
            IAppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToHome();
            }

            var entity = await _appService.Get(id);
            if (entity == null)
            {
                return Redirect("/user/apps");
            }

            ViewBag.IsActive = entity.IsActive;

            var model = AppModel.Map(entity);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> List(int id = 1)
        {
            var pageNumber = id;
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var apps = await _appService.GetApps(pageNumber);
            if (apps == null) return RedirectToHome();

            var list = apps.Items.Select(AppModel.Map).ToList();
            var model = new PageModel<AppModel>
            {
                Items = list,
                HasNextPage = apps.HasNextPage,
                HasPrevPage = apps.HasPreviousPage,
                PageNumber = apps.Number,
                ItemCount = apps.TotalCount,
                PageCount = apps.TotalPageCount
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult New()
        {
            var model = new AppModel();
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> New(AppModel model)
        {
            if (!model.IsValid())
            {
                SetPleaseTryAgain(model);
                return View(model);
            }

            model.CreatedBy = User.Identity.GetId();
            model.Email = User.Identity.GetEmail();
            model.Id = Guid.NewGuid().ToNoDashString();

            var isCreated = await _appService.CreateApp(model);
            if (isCreated) return Redirect(string.Format("/app/detail/{0}", model.Id));

            SetPleaseTryAgain(model);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> NewToken(string id)
        {
            var result = new BaseModel { IsOk = false };

            var token = new TokenModel
            {
                CreationDate = DateTime.Now,
                UsageCount = 0,
                Token = Guid.NewGuid().ToNoDashString(),
                AppId = id,
                CreatedBy = User.Identity.GetId()
            };

            var isOk = await _appService.CreateToken(token);
            if (!isOk) return Json(result, JsonRequestBehavior.DenyGet);

            result.IsOk = true;

            return Json(new { result.IsOk, result.Msg, token.Token, token.CreationDateStr, token.UsageCount }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> DeleteToken(string token)
        {
            var model = new BaseModel { IsOk = false };
            if (string.IsNullOrEmpty(token))
            {
                return Json(model, JsonRequestBehavior.DenyGet);
            }

            model.IsOk = await _appService.DeleteToken(token, User.Identity.GetId());

            return Json(model, JsonRequestBehavior.DenyGet);
        }

        [HttpGet]
        public async Task<JsonResult> NameControl(string name)
        {
            var model = new ResponseModel { IsOk = true };
            if (string.IsNullOrEmpty(name))
            {
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            var result = await _appService.GetByName(name);

            if (result == null) return Json(model, JsonRequestBehavior.AllowGet);

            while (result != null)
            {
                model.Msg = string.Format("{0}{1}", name, new Random().Next(1, 10));
                result = await _appService.GetByName(model.Msg);
            }

            model.IsOk = false;

            return Json(model, JsonRequestBehavior.AllowGet);
        }

    }
}