using System;
using System.Drawing.Imaging;

using NUnit.Framework;
using OpenQA.Selenium;

using set.messaging.test.Shared;

namespace set.messaging.test.Interface
{
    [TestFixture]
    public class FormTests : BaseInterfaceTest
    {
        [Test]
        public void should_add_new_app_and_redirect_to_app_detail()
        {
            LoginAsAdmin();

            var url = string.Format("{0}{1}", BASE_URL, ACTION_NEW_APP);
            var detailUrl = string.Format("{0}{1}", BASE_URL, "/app/detail");

            GoTo(url);

            Browser.FindElementById("Name").SendKeys("test app");
            Browser.FindElementById("Url").SendKeys("test.com");
            Browser.FindElementById("Description").SendKeys("test app description");

            Browser.FindElementById("frm").Submit();

            Assert.IsNotNull(Browser);
            Assert.IsTrue(Browser.Url.ToLowerInvariant().StartsWith(detailUrl));

            CloseBrowser();
        }

        [Test]
        public void should_add_new_token()
        {
            LoginAsAdmin();

            var url = string.Format("{0}{1}", BASE_URL, ACTION_APP_LISTING);

            GoTo(url);

            Browser.FindElement(By.CssSelector(".table > tbody:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > a:nth-child(1)")).Click();

            WaitHack();

            var count = Browser.FindElement(By.CssSelector("table tbody")).FindElements(By.TagName("tr")).Count;

            Browser.FindElementById("btnNewToken").Click();

            WaitHack();

            var newCount = Browser.FindElement(By.CssSelector("table tbody")).FindElements(By.TagName("tr")).Count;

            Assert.Greater(newCount, count);

            CloseBrowser();
        }

        [Test]
        public void should_delete_token()
        {
            try
            {
                LoginAsAdmin();

                var url = string.Format("{0}{1}", BASE_URL, ACTION_APP_LISTING);

                GoTo(url);

                Browser.FindElement(By.CssSelector(".table > tbody:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > a:nth-child(1)")).Click();

                var count = Browser.FindElement(By.CssSelector("table tbody")).FindElements(By.TagName("tr")).Count;

                Browser.FindElement(By.CssSelector(".table > tbody:nth-child(2) > tr:nth-child(1) > td:nth-child(1) > button:nth-child(1)")).Click();

                WaitHack();

                Browser.FindElementById("btnDelete").Click();

                WaitHack();

                var newCount = Browser.FindElement(By.CssSelector("table tbody")).FindElements(By.TagName("tr")).Count;

                Assert.Greater(count, newCount);

                CloseBrowser();
            }
            catch (ElementNotVisibleException ex)
            {
                
                
            }
        }

        private void WaitHack()
        {
            Browser.GetScreenshot().SaveAsFile(string.Format("{0}.png", Guid.NewGuid()), ImageFormat.Png);
            Browser.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
        }
    }
}