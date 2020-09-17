using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    public class GovIL
    {
        [Test]
        public void Test_32176_Trello_1744()
        {
            ClassicRunner runner = new ClassicRunner();

            var driver = new ChromeDriver();

            var eyes = new Eyes(runner);
            eyes.StitchMode = StitchModes.CSS;
            //eyes.ForceFullPageScreenshot = true;
            eyes.SetLogHandler(new StdoutLogHandler(true));

            try
            {
                eyes.Open(driver, "GovIL", "#32176", new Size(1600, 600));

                driver.Url = "https://govforms.gov.il/mw/forms/ComponentsDemo@test.gov.il";

                driver.ExecuteScript("document.documentElement.style.overflow='hidden'; document.documentElement.style.transform='translate(0px, 0px)';document.body.style.overflow='visible';");

                eyes.Check("Tab 1", Target.Region(By.CssSelector("#user")).Fully());

                driver.FindElement(By.CssSelector("#Tab2")).Click();
                eyes.Check("Tab 2", Target.Region(By.CssSelector("#user")).Fully());

                //driver.FindElement(By.CssSelector("#Tab3")).Click();
                //eyes.Check("Tab 3", Target.Region(By.CssSelector("#user")));

                //driver.FindElement(By.CssSelector("#Tab4")).Click();
                //eyes.Check("Tab 4", Target.Region(By.CssSelector("#user")));

                eyes.CloseAsync();
            }
            finally
            {
                driver.Quit();

                runner.GetAllTestResults();
            }
        }

    }
}
