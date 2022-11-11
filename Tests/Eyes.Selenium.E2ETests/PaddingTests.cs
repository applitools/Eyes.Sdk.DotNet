using System;
using Applitools;
using Applitools.Fluent;
using Applitools.Selenium;
using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Eyes.Selenium.E2ETests
{
    public class PaddingTests
    {
        private EyesRunner runner;
        private Applitools.Selenium.Eyes eyes;
        private IWebDriver driver;

        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");

        [SetUp]
        public void Setup()
        {
            var CI = Environment.GetEnvironmentVariable("CI");
            var options = new ChromeOptions();
            //  if (CI != null)
            {
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");
            }
            // Use Chrome browser
            driver = new ChromeDriver(DRIVER_PATH, options);
            
            // Initialize the eyes SDK (IMPORTANT: make sure your API key is set in the APPLITOOLS_API_KEY env variable).
            eyes = new Applitools.Selenium.Eyes();
            eyes.SaveNewTests = false;
            eyes.BranchName = "master";
            eyes.ParentBranchName = "master";
            eyes.HideScrollbars = true;
        }

        [TearDown]
        public void TearDown()
        {
            // Close the browser.
            driver.Quit();

            // If the test was aborted before eyes.close was called, ends the test as aborted.
            eyes.AbortIfNotClosed();
        }

        [Test]
        public void ShouldSendCoddedRegionsWithPadding()
        {
            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/PaddedBody/region-padding.html");
            driver = eyes.Open(driver, "Test Regions Padding", "TestRegionsPadding", new RectangleSize(width: 1100, height: 700));
            eyes.Check(Target.Window()
                .Ignore(By.CssSelector("#ignoreRegions"), new Padding(20, 20, 20, 20))
                .Layout(By.CssSelector("#layoutRegions"), new Padding(0, 20, 20, 0))
                .Content(By.CssSelector("#contentRegions"), new Padding(20, 0, 20, 0))
                .Strict(By.CssSelector("#strictRegions"), new Padding(0, 0, 0, 20))
                .Fully());
            var result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            CompareProcedureUtils.compareProcedure(info.ActualAppOutput[0].ImageMatchSettings.Ignore, new[] { new Region(131, 88, 838, 110, regionId: "#ignoreRegions") }, "ignore");
            CompareProcedureUtils.compareProcedure(info.ActualAppOutput[0].ImageMatchSettings.Layout, new[] { new Region(151, 238, 818, 90, regionId: "#layoutRegions") }, "layout");
            CompareProcedureUtils.compareProcedure(info.ActualAppOutput[0].ImageMatchSettings.Content, new[] { new Region(131, 408, 838, 70, regionId: "#contentRegions") }, "content");
            CompareProcedureUtils.compareProcedure(info.ActualAppOutput[0].ImageMatchSettings.Strict, new[] { new Region(151, 558, 798, 548, regionId: "#strictRegions") }, "strict");
        }
    }
}