using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Drawing;
using IEyesWebElement = Applitools.OpenQA.Selenium.IWebElement;

namespace Applitools.Selenium
{
    public class EyesWebElementImpl : IEyesWebElement, IWebElement
    {
        private IWebElement element_;

        public EyesWebElementImpl(IWebElement element)
        {
            element_ = element;
        }

        public string TagName => element_.TagName;

        public string Text => element_.Text;

        public bool Enabled => element_.Enabled;

        public bool Selected => element_.Selected;

        public Point Location => element_.Location;

        public Size Size => element_.Size;

        public bool Displayed => element_.Displayed;

        public void Clear()
        {
            element_.Clear();
        }

        public void Click()
        {
            element_.Click();
        }

        public IWebElement FindElement(By by)
        {
            return element_.FindElement(by);
        }

        public IEyesWebElement FindElement(OpenQA.Selenium.By by)
        {
            throw new System.NotImplementedException();
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return element_.FindElements(by);
        }

        public ReadOnlyCollection<IEyesWebElement> FindElements(OpenQA.Selenium.By by)
        {
            throw new System.NotImplementedException();
        }

        public string GetAttribute(string attributeName)
        {
            return element_.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return element_.GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            return element_.GetProperty(propertyName);
        }

        public void SendKeys(string text)
        {
            element_.SendKeys(text);
        }

        public void Submit()
        {
            element_.Submit();
        }
    }
}