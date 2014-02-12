using set.messaging.Helpers;

namespace set.messaging.Models
{
    public class LoginModel : BaseModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Password)
                   && Email.IsEmail();
        }
    }
}