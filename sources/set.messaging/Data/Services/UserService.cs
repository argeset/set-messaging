using System;
using System.Linq;
using System.Threading.Tasks;

using set.messaging.Data.Entities;
using set.messaging.Helpers;

namespace set.messaging.Data.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly IMessageService _messageService;

        public UserService(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public Task<User> Get(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;

            var user = Context.Users.FirstOrDefault(x => x.Id == userId);
            return Task.FromResult(user);
        }

        public Task<User> GetByEmail(string email)
        {
            if (!email.IsEmail()) return null;

            var user = Context.Users.FirstOrDefault(x => x.Email == email);
            return Task.FromResult(user);
        }

        public Task<bool> Authenticate(string email, string password)
        {
            if (!email.IsEmail() || string.IsNullOrEmpty(password)) return Task.FromResult(false);

            var user = Context.Users.FirstOrDefault(x => x.Email == email);
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

        public Task<bool> RequestPasswordReset(string email)
        {
            if (!email.IsEmail()) return Task.FromResult(false);

            var user = Context.Users.FirstOrDefault(x => x.IsActive
                                                         && !x.IsDeleted
                                                         && x.Email == email);

            if (user == null) return Task.FromResult(false);

            if (user.PasswordResetRequestedAt != null
               && user.PasswordResetRequestedAt.Value.AddMinutes(-1) > DateTime.Now) return Task.FromResult(false);

            var token = Guid.NewGuid().ToNoDashString();
            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;
            user.PasswordResetToken = token;
            user.PasswordResetRequestedAt = user.UpdatedAt;

            var saved = Context.SaveChanges() > 0;

            if (saved)
            {
                _messageService.SendEmail(
                                user.Email,
                                "password_reset_email_subject".Localize(),
                                string.Format("password_reset_email_body".Localize(), "password_reset_email_subject".Localize(), user.Name, user.Email, token)
                            );
            }

            return Task.FromResult(saved);
        }

        public Task<bool> IsPasswordResetRequestValid(string email, string token)
        {
            if (!email.IsEmail()) return Task.FromResult(false);
            var lifeTime = DateTime.Now.AddHours(-1);
            return Task.FromResult(Context.Users.Any(x => x.IsActive
                                                          && !x.IsDeleted
                                                          && x.Email == email
                                                          && x.PasswordResetToken == token
                                                          && x.PasswordResetRequestedAt >= lifeTime));
        }

        public async Task<bool> ChangePassword(string email, string token, string password)
        {
            if (!await IsPasswordResetRequestValid(email, token)) return await Task.FromResult(false);

            var user = Context.Users.FirstOrDefault(x => x.IsActive
                                                         && !x.IsDeleted
                                                         && x.Email == email);

            if (user == null) return await Task.FromResult(false);

            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = user.Id;
            user.PasswordResetToken = null;
            user.PasswordResetRequestedAt = null;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, 15);
            user.LoginTryCount = 0;

            return await Task.FromResult(Context.SaveChanges() > 0);
        }
    }

    public interface IUserService
    {
        Task<User> Get(string userId);
        Task<User> GetByEmail(string email);

        Task<bool> Authenticate(string email, string password);
        Task<bool> RequestPasswordReset(string email);
        Task<bool> IsPasswordResetRequestValid(string email, string token);
        Task<bool> ChangePassword(string email, string token, string password);
    }
}