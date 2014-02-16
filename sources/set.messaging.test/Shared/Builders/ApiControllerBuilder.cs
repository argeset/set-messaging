using Moq;

using set.messaging.Controllers;
using set.messaging.Data.Services;

namespace set.messaging.test.Shared.Builders
{
    public class ApiControllerBuilder : BaseBuilder
    {
        private IAppService _appService;
        private IMessageService _messageService;

        public ApiControllerBuilder()
        {
            _appService = new Mock<IAppService>().Object;
            _messageService = new Mock<IMessageService>().Object;
        }

        internal ApiControllerBuilder WithAppService(IAppService service)
        {
            _appService = service;
            return this;
        }

        internal ApiControllerBuilder WithMessageService(IMessageService service)
        {
            _messageService = service;
            return this;
        }

        internal ApiController Build()
        {
            var controller = new ApiController(_appService, _messageService)
            {
                ControllerContext = ControllerContext.Object
            };
            return controller;
        }
    }
}