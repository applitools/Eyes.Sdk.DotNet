using System.Collections.ObjectModel;

namespace Applitools.OpenQA.Selenium
{
    internal interface IFindsElement
    {
        IWebElement FindElement(string mechanism, string criteria);
        ReadOnlyCollection<IWebElement> FindElements(string mechanism, string criteria);
    }
}