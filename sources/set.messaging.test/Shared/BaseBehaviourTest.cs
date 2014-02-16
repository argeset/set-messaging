using System;
using System.Threading;

using set.messaging.Data.Entities;
using set.messaging.Helpers;
using set.messaging.Models;

namespace set.messaging.test.Shared
{
    public class BaseBehaviourTest
    {
        public const string ACTION_LOGIN = "Login";
        public const string ACTION_LOGOUT = "Logout";
        
        public const string ACTION_PASSWORD_RESET = "PasswordReset";
        public const string ACTION_PASSWORD_CHANGE = "PasswordChange";
        
        public const string EMAIL = "test@test.com";
        public const string PASSWORD = "pass";
        public const string NAME = "name";

        #region Valid Models
        public static readonly LoginModel ValidLoginModel = new LoginModel
        {
            Password = PASSWORD,
            Email = EMAIL
        };

        public static readonly UserModel ValidUserModel = new UserModel
        {
            Name = NAME,
            Password = PASSWORD,
            Email = EMAIL,
            Language = Thread.CurrentThread.CurrentUICulture.Name,
            Id = Guid.NewGuid().ToNoDashString()
        };

        public static readonly PasswordResetModel ValidPasswordResetModel = new PasswordResetModel
        {
            Email = EMAIL
        };

        public static readonly PasswordChangeModel ValidPasswordChangeModel = new PasswordChangeModel
        {
            Email = EMAIL,
            Password = PASSWORD,
            Token = "token"
        };
        #endregion

        #region Valid Entities
        public static readonly User ValidUserEntity = new User
        {
            Id = "1",
            Name = NAME,
            Email = EMAIL,
            RoleName = ConstHelper.User
        };
        #endregion
    }
}