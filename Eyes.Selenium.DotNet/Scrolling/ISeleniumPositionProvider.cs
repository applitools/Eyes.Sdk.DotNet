using OpenQA.Selenium;

namespace Applitools.Selenium.Scrolling
{
    internal interface ISeleniumPositionProvider
    {
        IWebElement ScrolledElement { get; }
    }
}