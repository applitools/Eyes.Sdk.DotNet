using OpenQA.Selenium;

namespace Eyes.Universal.Sdk.DotNet
{
    public static class WebDriverHelper
    {
        public static UniversalDriverConfig GetConfig(this IWebDriver webDriver)
        {
            return new UniversalDriverConfig
            {
                ServerUrl = webDriver.Url,
                //SessionId = webDriver.
            };
        }
    }
}