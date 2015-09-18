using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

using set.messaging.Data.Entities;
using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.Data.Services
{
    public class AppService : BaseService, IAppService
    {
        public Task<bool> CreateApp(AppModel model)
        {
            if (!model.IsValid()) return Task.FromResult(false);

            var user = Context.Set<User>().FirstOrDefault(x => x.Id == model.CreatedBy && x.IsActive);
            if (user == null) Task.FromResult(false);

            if (user.RoleName == ConstHelper.User) Task.FromResult(false);

            var key = Guid.NewGuid().ToNoDashString();
            var app = new App
            {
                Id = model.Id,
                UserId = user.Id,
                Name = model.Name,
                Url = model.Url,
                IsActive = true,
                CreatedBy = user.Id,
                Description = model.Description ?? string.Empty,
                Tokens = new List<Token>
                {
                    new Token
                    {
                        CreatedBy = user.Id,
                        Id = key,
                        UsageCount = 0,
                        IsAppActive = true,
                        IsActive = true
                    }
                }
            };

            Context.Set<App>().Add(app);

            return Task.FromResult(Context.SaveChanges() > 0);
        }

        public Task<PagedList<App>> GetApps(int pageNumber)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            var query = Context.Set<App>().Where(x => !x.IsDeleted);

            var count = query.Count();
            var items = query.OrderByDescending(x => x.CreatedBy).Skip(ConstHelper.PageSize * (pageNumber - 1)).Take(ConstHelper.PageSize).ToList();

            return Task.FromResult(new PagedList<App>(pageNumber, ConstHelper.PageSize, count, items));
        }

        public Task<App> Get(string appId)
        {
            if (string.IsNullOrEmpty(appId)) return null;

            var app = Context.Set<App>().Include(x => x.Tokens).FirstOrDefault(x => x.Id == appId);
            return Task.FromResult(app);
        }
        public Task<App> GetByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var app = Context.Apps.Include(x => x.Tokens).FirstOrDefault(x => x.Name == name);
            return Task.FromResult(app);
        }

        public Task<bool> CreateToken(TokenModel model)
        {
            if (!model.IsValid()) return Task.FromResult(false);

            var app = Context.Set<App>().FirstOrDefault(x => x.Id == model.AppId && x.UserId == model.CreatedBy);
            if (app == null) return Task.FromResult(false);

            var user = Context.Set<User>().FirstOrDefault(x => x.Id == model.CreatedBy);
            if (user == null) return Task.FromResult(false);

            if (user.RoleName == ConstHelper.User) return Task.FromResult(false);

            var entity = new Token
            {
                CreatedBy = user.Id,
                AppId = app.Id,
                Id = model.Token,
                UsageCount = 0,
                IsAppActive = true,
                IsActive = true
            };
            Context.Set<Token>().Add(entity);

            return Task.FromResult(Context.SaveChanges() > 0);
        }

        public Task<bool> DeleteToken(string token, string deletedBy)
        {
            if (string.IsNullOrEmpty(token)) return Task.FromResult(false);

            var item = Context.Set<Token>().FirstOrDefault(x => x.Id == token);
            if (item == null) return Task.FromResult(false);

            var user = Context.Set<User>().FirstOrDefault(x => x.Id == deletedBy);
            if (user == null) return Task.FromResult(false);

            if (user.RoleName == ConstHelper.User) return Task.FromResult(false);

            if (user.Id != item.CreatedBy
                && user.RoleName != ConstHelper.Admin) Task.FromResult(false);

            item.DeletedAt = DateTime.Now;
            item.DeletedBy = user.Id;
            item.IsDeleted = true;

            return Task.FromResult(Context.SaveChanges() > 0);
        }

        public Task<bool> IsTokenValid(string token)
        {
            return Task.FromResult(Context.Set<Token>().Any(x => x.Id == token
                                                                  && x.IsActive
                                                                  && x.IsAppActive
                                                                  && !x.IsDeleted));
        }

        public Task<bool> LogRequest(string token, string ip, string url)
        {
            if (string.IsNullOrEmpty(token)
                && string.IsNullOrEmpty(url)) return Task.FromResult(false);

            var tokenEntity = Context.Set<Token>().FirstOrDefault(x => x.Id == token);
            if (tokenEntity == null) return Task.FromResult(false);

            tokenEntity.UsageCount = tokenEntity.UsageCount + 1;

            var log = new RequestLog
            {
                Token = token,
                IP = ip,
                Url = url
            };
            Context.Set<RequestLog>().Add(log);

            return Task.FromResult(Context.SaveChanges() > 0);
        }
    }

    public interface IAppService
    {
        Task<bool> CreateApp(AppModel model);

        Task<PagedList<App>> GetApps(int pageNumber);

        Task<App> Get(string appId);
        Task<App> GetByName(string name);

        Task<bool> CreateToken(TokenModel token);
        Task<bool> DeleteToken(string token, string deletedBy);

        Task<bool> IsTokenValid(string token);

        /// <summary>
        /// logs the api requests
        /// </summary>
        /// <param name="token">the request token passed in http header</param>
        /// <param name="ip">the ip of the request</param>
        /// <param name="url">the url of the request</param>
        /// <returns></returns>
        Task<bool> LogRequest(string token, string ip, string url);
    }
}