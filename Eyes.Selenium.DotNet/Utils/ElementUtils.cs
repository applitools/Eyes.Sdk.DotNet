using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Applitools.Utils
{
    public static class ElementUtils
    {
        public static string GetElementId(this IWebElement webElement)
        {
            var fieldName = "elementId";
            if (webElement is RemoteWebElement remoteElement)
            {
                return remoteElement.GetBaseTypePrivateFieldValue<RemoteWebElement, string>(fieldName);
            }

            return webElement.GetPrivateFieldValue<string>(fieldName);
        }
    }
}
