using set.messaging.Helpers;

namespace set.messaging.Models
{
    public class PasswordChangeModel : BaseModel
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }

        public bool IsValid()
        {
            return Email.IsEmail() 
                   && !string.IsNullOrWhiteSpace(Token) 
                   && !string.IsNullOrWhiteSpace(Password);
        }

        public bool IsNotValid()
        {
            return !IsValid();
        }
    }
}