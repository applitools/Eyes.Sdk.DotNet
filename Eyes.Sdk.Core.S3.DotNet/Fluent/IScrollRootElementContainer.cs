using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools
{
    internal interface IScrollRootElementContainer
    {
        IWebElement GetScrollRootElement();
        By GetScrollRootSelector();
    }

    internal interface ITargetContainer
    {
        By GetTargetSelector();

        IWebElementWrapper GetTargetElement();
    }

    internal interface IWebElementWrapper
    {
        IWebElement WebElement { get; }
        string GetElementId();
    }

    internal class WebElementWrapper : IWebElementWrapper
    {
        public IWebElement WebElement { get; }

        public WebElementWrapper(IWebElement webElement)
        {
            WebElement = webElement;
        }

        public virtual string GetElementId()
        {
            return WebElement.GetPrivateFieldValue<string>("elementId");
        }
    }
}
