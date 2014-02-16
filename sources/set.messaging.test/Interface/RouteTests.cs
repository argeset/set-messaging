using NUnit.Framework;
using set.messaging.test.Shared;

namespace set.messaging.test.Interface
{
    [TestFixture]
    public class RouteTests : BaseInterfaceTest
    {
        [TestCase(ACTION_HOME),
         TestCase(ACTION_LOGIN),
         TestCase(ACTION_PASSWORD_RESET)]
        public void should_view(string view)
        {
            var url = string.Format("{0}{1}", BASE_URL, view);

            GoTo(url);
            AssertUrl(url);

            CloseBrowser();
        }

        [TestCase(ACTION_APP_LISTING),
         TestCase(ACTION_NEW_APP)]
        public void should_redirect_to_login_if_requested_anonymous(string view)
        {
            LogOut();

            var url = string.Format("{0}{1}", BASE_URL, view);
            var loginUrl = string.Format("{0}{1}", BASE_URL, ACTION_LOGIN);

            GoTo(url);

            Assert.IsNotNull(Browser);
            Assert.True(Browser.Url.ToLowerInvariant().StartsWith(loginUrl));

            CloseBrowser();
        }

        [TestCase(ACTION_APP_LISTING),
         TestCase(ACTION_NEW_APP)]
        public void should_view_after_login_as_admin(string view)
        {
            LoginAsAdmin();

            var url = string.Format("{0}{1}", BASE_URL, view);

            GoTo(url);
            AssertUrl(url);

            CloseBrowser();
        }
    }
}