using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Applitools.Utils
{
    public static class ElementUtils
    {
        public static string GetElementId(this IWebElement webElement)
        {
            var fieldName = "elementId";
            var remoteElement = webElement as RemoteWebElement;
            if (remoteElement != null)
            {
                return remoteElement.GetBaseTypePrivateFieldValue<RemoteWebElement, string>(fieldName);
            }

            return webElement.GetPrivateFieldValue<string>(fieldName);
        }
    }
}
