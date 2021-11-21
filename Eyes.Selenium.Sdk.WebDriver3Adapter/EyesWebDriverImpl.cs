using OpenQA.Selenium;
using System.Collections.ObjectModel;
using IEyesWebDriver = Applitools.OpenQA.Selenium.IWebDriver;
using IEyesWebElement = Applitools.OpenQA.Selenium.IWebElement;
using IEyesOptions = Applitools.OpenQA.Selenium.IOptions;
using IEyesNavigation = Applitools.OpenQA.Selenium.INavigation;
using IEyesTargetLocator = Applitools.OpenQA.Selenium.ITargetLocator;
using IEyesCapabilities = Applitools.OpenQA.Selenium.ICapabilities;
using EyesBy = Applitools.OpenQA.Selenium.By;
using OpenQA.Selenium.Internal;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;

namespace Applitools.Selenium
{
    internal class EyesWebDriverImpl : IEyesWebDriver, IWebDriver
    {
        private readonly RemoteWebDriver driver_;

        public EyesWebDriverImpl(IWebDriver driver)
        {
            if (driver is RemoteWebDriver rwd)
            {
                driver_ = rwd;
            }
            else
            {
                throw new EyesException("driver must be a RemoteWebDriver");
            }
        }

        public string Url { get => driver_.Url; set => driver_.Url = value; }

        public string Title => driver_.Title;

        public string PageSource => driver_.PageSource;

        public string CurrentWindowHandle => driver_.CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => driver_.WindowHandles;

        public string SessionId => ((RemoteWebDriver)driver_).SessionId.ToString();

        public IEyesCapabilities Capabilities => throw new System.NotImplementedException();

        public bool IsActionExecutor => ((IActionExecutor)driver_).IsActionExecutor;

        public void Close()
        {
            driver_.Close();
        }

        public void Dispose()
        {
            driver_.Dispose();
        }

        public object ExecuteAsyncScript(string script, object[] args)
        {
            return ((IJavaScriptExecutor)driver_).ExecuteScript(script, args);
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)driver_).ExecuteScript(script, args);
        }

        IWebElement ISearchContext.FindElement(By by)
        {
            return driver_.FindElement(by);
        }

        public IEyesWebElement FindElement(EyesBy eyesBy)
        {
            By by = eyesBy.ToBy();
            IWebElement element = driver_.FindElement(by);
            EyesWebElementImpl eyesWebElement = new EyesWebElementImpl(element);
            return eyesWebElement;
        }

        ReadOnlyCollection<IWebElement> ISearchContext.FindElements(By by)
        {
            return driver_.FindElements(by);
        }

        public ReadOnlyCollection<IEyesWebElement> FindElements(EyesBy eyesBy)
        {
            By by = eyesBy.ToBy();
            ReadOnlyCollection<IWebElement> elements = driver_.FindElements(by);
            List<IEyesWebElement> result = new List<IEyesWebElement>(elements.Count);
            foreach (IWebElement e in elements)
            {
                result.Add(new EyesWebElementImpl(e));
            }
            ReadOnlyCollection<IEyesWebElement> roResult = new ReadOnlyCollection<IEyesWebElement>(result);
            return roResult;
        }

        public IOptions Manage()
        {
            return driver_.Manage();
        }

        public INavigation Navigate()
        {
            return driver_.Navigate();
        }

        public void Quit()
        {
            driver_.Quit();
        }

        public void ResetInputState()
        {
            ((IActionExecutor)driver_).ResetInputState();
        }

        public ITargetLocator SwitchTo()
        {
            return driver_.SwitchTo();
        }

        IEyesOptions IEyesWebDriver.Manage()
        {
            throw new System.NotImplementedException();
        }

        IEyesNavigation IEyesWebDriver.Navigate()
        {
            throw new System.NotImplementedException();
        }

        IEyesTargetLocator IEyesWebDriver.SwitchTo()
        {
            throw new System.NotImplementedException();
        }
    }
}