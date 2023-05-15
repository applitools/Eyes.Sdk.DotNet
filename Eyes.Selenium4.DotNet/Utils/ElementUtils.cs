using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Applitools.Utils
{
    public static class ElementUtils
    {
        public static string GetElementId(this IWebElement webElement)
        {
            var fieldName = "elementId";
            if (webElement is WebElement remoteElement)
            {
                return remoteElement.GetBaseTypePrivateFieldValue<WebElement, string>(fieldName);
            }

            return webElement.GetPrivateFieldValue<string>(fieldName);
        }
    }
}
