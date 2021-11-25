namespace Applitools.Selenium.Common
{
    internal interface IPageStateCommon<IWebDriver, IWebElement>
    {
        void PreparePage(ISeleniumCheckTargetCommon<IWebDriver, IWebElement> seleniumCheckTarget, Configuration config, IWebElement userDefinedSRE);
    }
}