using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Amazon.SimpleEmail.Model;
using Moq;
using NUnit.Framework;

using set.messaging.Data.Services;
using set.messaging.Helpers;
using set.messaging.test.Shared;
using set.messaging.test.Shared.Builders;

namespace set.messaging.test.Behaviour
{
    [TestFixture]
    public class ApiBehaviourTests
    {
        [Test]
        public async void should_send_email()
        {
            //arrange  
            var controllerContext = new Mock<ControllerContext>();
            var httpContext = new Mock<HttpContextBase>();
            var httpRequest = new Mock<HttpRequestBase>();

            controllerContext.Setup(x => x.HttpContext).Returns(httpContext.Object);
            httpContext.Setup(x => x.Request).Returns(httpRequest.Object);

            var headers = new NameValueCollection { { ConstHelper.Authorization, "token" } };
            httpRequest.Setup(x => x.Headers).Returns(headers);

            const string actionName = "SendEmail";
            const string email = "to@to.com";
            const string subject = "subject";
            const string htmlBody = "htmlBody";

            var messageService = new Mock<IMessageService>();
            messageService.Setup(x => x.SendEmail(email, subject, htmlBody)).Returns(Task.FromResult(new SendEmailResponse()));
            
            //act
            var sut = new ApiControllerBuilder().WithMessageService(messageService.Object).Build();
            sut.ControllerContext = controllerContext.Object;

            var result = await sut.SendEmail(email, subject, htmlBody);
            
            //assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<JsonResult>(result);

            messageService.Verify(x => x.SendEmail(email, subject, htmlBody), Times.Once);

            sut.AssertPostAttributeWithOutAntiForgeryToken(actionName, new[] { typeof(string), typeof(string), typeof(string) });
            sut.AssertValidateInputAttribute(actionName, new[] { typeof(string), typeof(string), typeof(string) });
        }
    }
}