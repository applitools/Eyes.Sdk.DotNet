using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestBadSelectors : ReportingTestSuite
    {
        [TestCase(false)]
        [TestCase(true)]
        public void TestCheckRegionWithBadSelector(bool useVisualGrid)
        {
            SeleniumUtils.InitEyes(useVisualGrid, out IWebDriver driver, out EyesRunner runner, out Eyes eyes);
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            string suffix = useVisualGrid ? "_VG" : "";
            eyes.Open(driver, "Applitools Eyes DotNet SDK",
                nameof(TestCheckRegionWithBadSelector) + suffix,
                new Size(1200, 800));

            try
            {
                Assert.That(() =>
                {
                    eyes.CheckRegion(By.CssSelector("#element_that_does_not_exist"));

                    eyes.CloseAsync();
                    runner.GetAllTestResults();

                }, Throws.Exception.With.InstanceOf<EyesException>().With.InnerException.With.InstanceOf<NoSuchElementException>());
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestCheckRegionWithBadIgnoreSelector(bool useVisualGrid)
        {
            SeleniumUtils.InitEyes(useVisualGrid, out IWebDriver driver, out EyesRunner runner, out Eyes eyes);
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            string suffix = useVisualGrid ? "_VG" : "";
            eyes.Open(driver, "Applitools Eyes DotNet SDK", nameof(TestCheckRegionWithBadIgnoreSelector) + suffix, new Size(1200, 800));

            eyes.Check(Target.Window().Ignore(By.CssSelector("body>p:nth-of-type(14)"))
                .BeforeRenderScreenshotHook("var p = document.querySelector('body>p:nth-of-type(14)'); p.parentNode.removeChild(p);"));

            try
            {
                eyes.Close();
                runner.GetAllTestResults();
            }
            finally
            {
                driver.Quit();
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public void TestCheckRegionWithBadSelectorBeforeValidCheck(bool useVisualGrid)
        {
            SeleniumUtils.InitEyes(useVisualGrid, out IWebDriver driver, out EyesRunner runner, out Eyes eyes);
            driver.Url = "https://applitools.github.io/demo/TestPages/VisualGridTestPage/";

            string suffix = useVisualGrid ? "_VG" : "";
            eyes.Open(driver, "Applitools Eyes DotNet SDK", nameof(TestCheckRegionWithBadSelectorBeforeValidCheck) + suffix, new Size(1200, 800));

            try
            {
                Assert.That(() =>
                {
                    eyes.CheckRegion(By.CssSelector("#element_that_does_not_exist"));
                    driver.FindElement(By.Id("centered")).Click();
                    eyes.CheckRegion(By.Id("modal-content"));

                    eyes.CloseAsync();
                    runner.GetAllTestResults();

                }, Throws.Exception.With.InstanceOf<NoSuchElementException>().Or.With.InstanceOf<EyesException>().With.InnerException.With.InstanceOf<NoSuchElementException>());
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
                runner.GetAllTestResults();
            }
        }
    }
}
