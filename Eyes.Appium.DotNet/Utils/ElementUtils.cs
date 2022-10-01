using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Applitools.Appium.Utils
{
    public static class ElementUtils
    {
        public static string GetElementId(this IWebElement webElement)
        {
            if (webElement is AppiumWebElement element)
            {
                return element.Id;
            }

            return webElement.GetPrivateFieldValue<string>("elementId");
        }
    }
}
