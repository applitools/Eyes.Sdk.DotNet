using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using IEyesWebElement = Applitools.OpenQA.Selenium.IWebElement;

namespace Applitools.Selenium
{
    public class EyesWebDriverAdapter : IEyesWebDriverAdapter
    {
        private readonly MethodInfo executeCommandMI_;
        private readonly PropertyInfo elementIdPropertyInfo_;
        private readonly ICommandExecutor executor_;
        private readonly IWebDriver driver_;

        public EyesWebDriverAdapter(IWebDriver driver)
        {
            driver_ = driver;
            executeCommandMI_ = driver.GetType().GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
            PropertyInfo pi = typeof(RemoteWebDriver).GetProperty("CommandExecutor", BindingFlags.NonPublic | BindingFlags.Instance);
            elementIdPropertyInfo_ = typeof(RemoteWebElement).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance);
            executor_ = (ICommandExecutor)pi.GetValue(driver_);
        }

        public string GetElementId(Logger logger, IEyesWebElement webElement)
        {
            if (webElement is RemoteWebElement)
            {
                string id = (string)elementIdPropertyInfo_.GetValue(webElement);
                return id;
            }
            string errMsg = $"The input web element is not a WebElement ({webElement.GetType().Name})";
            logger.Log(TraceLevel.Error, Stage.General, new { errMsg });
            throw new EyesException(errMsg);
        }

        public IDictionary<string, object> GetSessionDetails()
        {
            try
            {
                var response = (Response)executeCommandMI_.Invoke(driver_, new object[] { "getSession", null });
                if (response == null || response.Status != WebDriverResult.Success) return null;
                if (!(response.Value is IDictionary<string, object> dict)) return null;
                return dict.Where(entry =>
                {
                    string key = entry.Key;
                    object value = entry.Value;
                    return !string.IsNullOrEmpty(key) && value != null && !string.IsNullOrEmpty(Convert.ToString(value));
                })
                .ToDictionary(entry => entry.Key, entry => entry.Value);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetTagName(Logger logger, IEyesWebElement webElement)
        {
            if (webElement is IWebElement element)
            {
                return element.TagName;
            }
            if (webElement is EyesWebElement eyesWebElement)
            {
                return eyesWebElement.TagName;
            }
            return null;
        }

        public void TryAddCommand(string commandName, string method, string resourcePath)
        {
            executor_.CommandInfoRepository.TryAddCommand("getSession", new CommandInfo("GET", "/session/{sessionId}/"));
        }

        public EyesWebElement WrapElementIfRemote(Logger logger, EyesWebDriver driver, IEyesWebElement elementToWrap)
        {
            return elementToWrap is EyesWebElement element ? element : new EyesWebElement(logger, driver, elementToWrap);
        }
    }
}
