using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using set.messaging.Data.Services;
using set.messaging.Helpers;
using set.messaging.Models;
using set.messaging.test.Shared;
using set.messaging.test.Shared.Builders;

namespace set.messaging.test.Behaviour
{
    [TestFixture]
    public class MembershipBehaviourTests : BaseBehaviourTest
    { 
        [Test]
        public async void any_user_can_login()
        {
            //arrange  
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.Authenticate(ValidLoginModel.Email, ValidLoginModel.Password))
                       .Returns(Task.FromResult(true));

            userService.Setup(x => x.GetByEmail(ValidLoginModel.Email))
                       .Returns(Task.FromResult(ValidUserEntity));

            var authService = new Mock<IAuthService>();
            authService.Setup(x => x.SignIn(ValidUserEntity.Id, ValidUserEntity.Name, ValidUserEntity.Email, ConstHelper.User, true));

            ////act
            var sut = new UserControllerBuilder().WithUserService(userService.Object)
                                                 .WithAuthService(authService.Object)
                                                 .Build();

            var result = await sut.Login(ValidLoginModel);

            ////assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<RedirectResult>(result);

            userService.Verify(x => x.Authenticate(ValidLoginModel.Email, ValidLoginModel.Password), Times.Once);
            userService.Verify(x => x.GetByEmail(ValidLoginModel.Email), Times.Once);
            authService.Verify(x => x.SignIn(ValidUserEntity.Id, ValidUserEntity.Name, ValidUserEntity.Email, ConstHelper.User, true), Times.Once);

            sut.AssertPostAttribute(ACTION_LOGIN, new[] { typeof(LoginModel) });
            sut.AssertAllowAnonymousAttribute(ACTION_LOGIN, new[] { typeof(LoginModel) });
        }

        [Test]
        public void any_user_can_logout()
        {
            //arrange
            var authService = new Mock<IAuthService>();
            authService.Setup(x => x.SignOut());

            //act
            var sut = new UserControllerBuilder().WithAuthService(authService.Object)
                                                 .Build();
            var result = sut.Logout();

            //assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<RedirectResult>(result);

            sut.AssertGetAttribute(ACTION_LOGOUT); 
        }

        [Test]
        public async void any_user_can_request_password_reset_link()
        {
            //arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.RequestPasswordReset(ValidPasswordResetModel.Email))
                       .Returns(Task.FromResult(true));

            //act
            var sut = new UserControllerBuilder().WithUserService(userService.Object)
                                                 .Build();
            var result = await sut.PasswordReset(ValidPasswordResetModel);
            
            //assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<ViewResult>(result);

            userService.Verify(x => x.RequestPasswordReset(ValidPasswordResetModel.Email), Times.Once);

            sut.AssertPostAttribute(ACTION_PASSWORD_RESET, new[] {typeof (PasswordResetModel)});
            sut.AssertAllowAnonymousAttribute(ACTION_PASSWORD_RESET, new[] { typeof(PasswordResetModel) });
        }

        [Test]
        public async void any_user_can_change_password()
        {
            //arrange
            var userService = new Mock<IUserService>();
            userService.Setup(x => x.ChangePassword(ValidPasswordChangeModel.Email, ValidPasswordChangeModel.Token, ValidPasswordChangeModel.Password))
                       .Returns(Task.FromResult(true));

            //act
            var sut = new UserControllerBuilder().WithUserService(userService.Object)
                                                 .Build();
            var result = await sut.PasswordChange(ValidPasswordChangeModel);

            //assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<RedirectResult>(result);

            userService.Verify(x => x.ChangePassword(ValidPasswordChangeModel.Email, ValidPasswordChangeModel.Token, ValidPasswordChangeModel.Password), Times.Once);

            sut.AssertPostAttributeWithOutAntiForgeryToken(ACTION_PASSWORD_CHANGE, new[] { typeof(PasswordChangeModel) });
            sut.AssertAllowAnonymousAttribute(ACTION_PASSWORD_CHANGE, new[] { typeof(PasswordChangeModel) });
        }
    }
}