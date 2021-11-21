using Applitools.Selenium;
using OpenQA.Selenium;
using System;

namespace Applitools
{
    public class Eyes : EyesImpl
    {
        public Eyes(ILogHandler logHandler = null) : base(logHandler) { }
        public Eyes(Uri serverUrl, ILogHandler logHandler = null) : base(serverUrl, logHandler) { }
        public Eyes(EyesRunner runner) : base(runner) { }

        public IWebDriver Open(IWebDriver driver)
        {
            EyesWebDriverImpl driverAdapter = new EyesWebDriverImpl(driver);
            base.Open(driverAdapter);
            return driverAdapter;
        }
    }


}
