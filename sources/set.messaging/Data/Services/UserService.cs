using System;
using System.Linq;
using System.Threading.Tasks;

using set.messaging.Data.Entities;
using set.messaging.Helpers;

namespace set.messaging.Data.Services
{
    public class UserService : BaseService, IUserService
    {
        public Task<User> Get(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var user = Context.Set<User>().FirstOrDefault(x => x.Id == userId);
            return Task.FromResult(user);
        }
        
        public Task<User> GetByEmail(string email)
        {
            if (!email.IsEmail()) return null;

            var user = Context.Set<User>().FirstOrDefault(x => x.Email == email);
            return Task.FromResult(user);
        }


        public Task<bool> Authenticate(string email, string password)
        {
            if (!email.IsEmail() || string.IsNullOrEmpty(password)) return Task.FromResult(false);

            var user = Context.Set<User>().FirstOrDefault(x => x.Email == email);
            if (user == null) return Task.FromResult(false);

            var result = false;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)
                && user.LoginTryCount < 5)
            {
                user.LastLoginAt = DateTime.Now;
                user.LoginTryCount = 0;
                result = true;
            }
            else
            {
                user.LoginTryCount += 1;
            }

            Context.SaveChanges();

            return Task.FromResult(result);
        }
    }

    public interface IUserService
    {
        Task<User> Get(string userId);
        Task<User> GetByEmail(string email);

        Task<bool> Authenticate(string email, string password);
    }
}