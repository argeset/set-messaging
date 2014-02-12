using set.messaging.Data.Entities;
using set.messaging.Helpers;

namespace set.messaging.Models
{
    public class UserModel : BaseModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public string Language { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Password)
                   && !string.IsNullOrEmpty(Name)
                   && Email.IsEmail();
        }

        public static UserModel Map(User user)
        {
            var model = new UserModel
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                RoleName = user.RoleName,
                Language = user.Language,
                IsActive = user.IsActive,
                RoleId = user.RoleId
            };
            return model;
        }
    }
}