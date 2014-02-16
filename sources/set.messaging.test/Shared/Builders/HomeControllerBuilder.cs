using set.messaging.Controllers;

namespace set.messaging.test.Shared.Builders
{
    public class HomeControllerBuilder : BaseBuilder
    {
        internal HomeController Build()
        {
            return new HomeController();
        } 
    }
}