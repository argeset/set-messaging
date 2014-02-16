using System.Threading.Tasks;
using System.Web.Mvc;

using Amazon.SimpleEmail.Model;
using Moq;
using NUnit.Framework;

using set.messaging.Data.Services;
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
            var messageService = new Mock<IMessageService>();
            messageService.Setup(x => x.SendEmail("to", "subject", "htmlBody")).Returns(Task.FromResult(new SendEmailResponse()));

           

            //act
            var sut = new ApiControllerBuilder().WithMessageService(messageService.Object).Build();
            

            var result = await sut.SendMail("to", "subject", "htmlBody");

            //assert
            Assert.IsNotNull(result);
            Assert.IsAssignableFrom<JsonResult>(result);

            messageService.Verify(x => x.SendEmail("to", "subject", "htmlBody"), Times.Once);

            sut.AssertPostAttributeWithOutAntiForgeryToken("SendEmail", new[] { typeof(string), typeof(string), typeof(string) });
        }
    }
}