using System.Collections.Generic;

namespace Applitools.Selenium.Common
{
    internal interface ISeleniumCheckTargetCommon
    {
        List<int> GetLayoutBreakpoints();

        bool GetLayoutBreakpointsEnabled();
        
        bool? GetUseCookies();
    }
}
