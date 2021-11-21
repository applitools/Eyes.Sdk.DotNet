using Applitools.Selenium;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Applitools
{
    public class Eyes : EyesImpl
    {
        public new IWebDriver Open(IWebDriver driver)
        {
            EyesWebDriver wrappingDriver = new EyesWebDriver(driver);
            base.Open(wrappingDriver);
        }
    }


}
