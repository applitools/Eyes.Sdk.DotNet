using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Eyes.Appium.DotNet.Utils
{
    public static class ElementUtils
    {
        public static string GetElementId(this IWebElement webElement, string fieldName)
        {
            if (webElement is AppiumWebElement element)
            {
                return element.Id;
            }

            return webElement.GetPrivateFieldValue<string>("elementId");
        }
    }
}
