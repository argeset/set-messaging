using System;
using System.Linq;
using System.Web.Mvc;

using NUnit.Framework;

namespace set.messaging.test.Shared
{
    public static class ControllerTestHelper
    {
        private static void AssertAttribute(Controller controller, string actionMethodName, Type attribute, Type[] parameterTypes)
        {
            var type = controller.GetType();
            var methodInfo = type.GetMethod(actionMethodName, parameterTypes ?? new Type[0]);
            var attributes = methodInfo.GetCustomAttributes(attribute, true);

            Assert.IsTrue(attributes.Any(), string.Format("{0} not found on action {1}", attribute.Name, actionMethodName));
        }

        public static void AssertGetAttribute(this Controller controller, string actionMethodName, Type[] parameterTypes = null)
        {
            AssertAttribute(controller, actionMethodName, typeof(HttpGetAttribute), parameterTypes);
        }

        public static void AssertAllowAnonymousAttribute(this Controller controller, string actionMethodName, Type[] parameterTypes = null)
        {
            AssertAttribute(controller, actionMethodName, typeof(AllowAnonymousAttribute), parameterTypes);
        }
         
        public static void AssertPostAttribute(this Controller controller, string actionMethodName, Type[] parameterTypes = null)
        {
            AssertAttribute(controller, actionMethodName, typeof(HttpPostAttribute), parameterTypes);
            AssertAttribute(controller, actionMethodName, typeof(ValidateAntiForgeryTokenAttribute), parameterTypes);
        }

        public static void AssertPostAttributeWithOutAntiForgeryToken(this Controller controller, string actionMethodName, Type[] parameterTypes = null)
        {
            AssertAttribute(controller, actionMethodName, typeof(HttpPostAttribute), parameterTypes);
        } 
    }
}
