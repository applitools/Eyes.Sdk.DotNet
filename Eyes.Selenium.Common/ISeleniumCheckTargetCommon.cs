using System.Collections.Generic;

namespace Applitools.Selenium.Common
{
    internal interface ISeleniumCheckTargetCommon<IWebDriver, IWebElement>
    {
        List<int> GetLayoutBreakpoints();

        bool GetLayoutBreakpointsEnabled();
        
        bool? GetUseCookies();
        
        CheckState<IWebElement> State { get; set; }

        void SanitizeSettings(Logger logger, IWebDriver driver, CheckState<IWebElement> state);
    }
}
