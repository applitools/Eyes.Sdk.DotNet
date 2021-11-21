using System.Collections.ObjectModel;
using System.Drawing;

namespace Applitools.OpenQA.Selenium
{
    public interface IWebElement
    {
        Point Location { get; set; }

        Size Size { get; set; }

        string TagName { get; }
        string Text { get; }
        bool Enabled { get; }
        bool Selected { get; }
        bool Displayed { get; }

        void SendKeys(string text);
        void Click();
        void Clear();
        string GetAttribute(string attributeName);
        IWebElement FindElement(By by);
        ReadOnlyCollection<IWebElement> FindElements(By by);
        void Submit();
        string GetProperty(string propertyName);
        string GetCssValue(string propertyName);
    }
}
