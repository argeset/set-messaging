using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

using Moq;

namespace set.messaging.test.Shared.Builders
{
    public class BaseBuilder
    {
        public Mock<ControllerContext> ControllerContext = new Mock<ControllerContext>();
        public Mock<HttpContextBase> HttpContext = new Mock<HttpContextBase>();
        public Mock<HttpRequestBase> HttpRequest = new Mock<HttpRequestBase>();
        public Mock<HttpResponseBase> HttpResponse = new Mock<HttpResponseBase>();

        public Mock<IPrincipal> User = new Mock<IPrincipal>();
        public Mock<IIdentity> CurrentUser = new Mock<IIdentity>();

        public void SetCurrentUser(string id, string name, string email, string role)
        {
            ControllerContext.Setup(x => x.HttpContext).Returns(HttpContext.Object);

            HttpContext.Setup(x => x.Request).Returns(HttpRequest.Object);
            HttpContext.Setup(x => x.Response).Returns(HttpResponse.Object);
            HttpContext.Setup(x => x.User).Returns(User.Object);

            User.Setup(x => x.Identity).Returns(CurrentUser.Object);

            CurrentUser.Setup(x => x.IsAuthenticated).Returns(true);
            CurrentUser.Setup(x => x.Name).Returns(string.Format("{0}|{1}|{2}|{3}", id, name, email, role));

            HttpResponse.Setup(x => x.SetCookie(It.IsAny<HttpCookie>()));

            HttpRequest.Setup(x => x.RequestContext.HttpContext.Request).Returns(HttpRequest.Object);
            HttpRequest.Setup(x => x.Headers).Returns(new NameValueCollection());
        }  
    }
}