using OpenQA.Selenium;

namespace Applitools.Selenium
{
    internal class SeleniumJavaScriptExecutor : Utils.IEyesJsExecutor
    {
        private IJavaScriptExecutor jsExecutor_;

        public SeleniumJavaScriptExecutor(IWebDriver driver)
        {
            jsExecutor_ = (IJavaScriptExecutor)driver;
        }

        public SeleniumJavaScriptExecutor(EyesWebDriver driver)
        {
            jsExecutor_ = driver.RemoteWebDriver;
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return jsExecutor_.ExecuteScript(script, args);
        }
    }
}