using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.Mock
{
    public class WebDriverProvider
    {
        private IWebDriver driver_;

        public void SetDriver(IWebDriver driver)
        {
            driver_ = driver;
        }

        public IWebDriver ProvideDriver()
        {
            return driver_;
        }
    }
}