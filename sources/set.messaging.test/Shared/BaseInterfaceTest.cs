using NUnit.Framework;
using OpenQA.Selenium.Firefox;

namespace set.messaging.test.Shared
{
    public class BaseInterfaceTest
    {
        public const string BASE_URL = "http://localhost:8022";

        public const string ACTION_HOME = "/";

        public const string ACTION_LOGIN = "/user/login";
        public const string ACTION_LOGOUT = "/user/logout";
        public const string ACTION_PASSWORD_RESET = "/user/passwordreset";
        
        public const string ACTION_APP_LISTING = "/app/list";
        public const string ACTION_NEW_APP = "/app/new";
        

        public FirefoxDriver Browser;

        [SetUp]
        public void Setup()
        {
            Browser = new FirefoxDriver();
        }

        public void LoginAsAdmin()
        {
            LogOut();

            GoTo(string.Format("{0}{1}", BASE_URL, ACTION_LOGIN));

            Browser.FindElementById("Email").SendKeys("admin@test.com");
            Browser.FindElementById("Password").SendKeys("password");
            Browser.FindElementById("frm").Submit();
        }

        public void LogOut()
        {
            GoTo(string.Format("{0}{1}", BASE_URL, ACTION_LOGOUT));
        }

        public void GoTo(string url)
        {
            Browser.Navigate().GoToUrl(url);
        }

        public void AssertUrl(string url)
        {
            Assert.IsNotNull(Browser);
            Assert.AreEqual(Browser.Url, url);
        }

        public void CloseBrowser()
        {
            Browser.Close();
        }
    }
}