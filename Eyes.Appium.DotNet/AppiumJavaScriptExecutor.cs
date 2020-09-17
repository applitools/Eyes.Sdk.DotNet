using OpenQA.Selenium.Remote;

namespace Applitools.Appium
{
    internal class AppiumJavaScriptExecutor : Utils.IEyesJsExecutor
    {
        private RemoteWebDriver driver_;

        public AppiumJavaScriptExecutor(RemoteWebDriver driver)
        {
            driver_ = driver;
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return driver_.ExecuteScript(script, args);
        }
    }
}