using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Applitools.Selenium
{
    public class EyesWebDriverAdapter : IEyesWebDriverAdapter
    {
        private readonly MethodInfo executeCommandMI_;
        private readonly ICommandExecutor executor_;
        private readonly PropertyInfo elementIdPropertyInfo_;
        private readonly IWebDriver driver_;

        public EyesWebDriverAdapter(IWebDriver driver)
        {
            driver_ = driver;
            executor_ = ((IHasCommandExecutor)driver).CommandExecutor;
            elementIdPropertyInfo_ = typeof(WebElement).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string GetElementId(Logger logger, IWebElement webElement)
        {
            string id = (string)elementIdPropertyInfo_.GetValue(webElement);
            return id;
        }

        public IDictionary<string, object> GetSessionDetails()
        {
            Type t = ((IHasCapabilities)driver_).Capabilities.GetType();
            var mi = t.GetMethod("ToDictionary");
            if (mi != null)
            {
                return (IDictionary<string, object>)mi.Invoke(driver_, null);
            }
            var fi = t.GetField("capabilities");
            if (fi != null)
            {
                return (IDictionary<string, object>)fi.GetValue(driver_);
            }
            return new Dictionary<string, object>();
        }

        public void TryAddCommand(string commandName, string method, string resourcePath)
        {
            executor_.TryAddCommand("getSession", new HttpCommandInfo("GET", "/session/{sessionId}/"));
        }

        public IWebElement WrapElementIfRemote(Logger logger, EyesWebDriver driver, IWebElement elementToWrap)
        {
            if (elementToWrap is WebElement)
            {
                return new EyesWebElement(logger, driver, elementToWrap);
            }

            return elementToWrap;
        }
    }
}
