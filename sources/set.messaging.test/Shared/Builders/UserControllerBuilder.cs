using System.Web;

using Moq;

using set.messaging.Controllers;
using set.messaging.Data.Services;

namespace set.messaging.test.Shared.Builders
{
    public class UserControllerBuilder : BaseBuilder
    {
        private IUserService _userService;
        private IAuthService _authService;

        public UserControllerBuilder()
        {
            _authService = new Mock<IAuthService>().Object;
            _userService = new Mock<IUserService>().Object;
        }

        internal UserControllerBuilder WithAuthService(IAuthService authService)
        {
            _authService = authService;
            return this;
        }

        internal UserControllerBuilder WithUserService(IUserService userService)
        {
            _userService = userService;
            return this;
        }

        internal UserController BuildWithMockControllerContext(string id, string name, string email, string role)
        {
            var sut = Build();

            SetCurrentUser(id,name,email,role);

            HttpResponse.Setup(x => x.SetCookie(It.IsAny<HttpCookie>()));

            sut.ControllerContext = ControllerContext.Object;
            return sut;
        }

        internal UserController Build()
        {
            return new UserController(_authService, _userService);
        }
    }
}