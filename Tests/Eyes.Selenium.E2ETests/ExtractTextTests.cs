using System;
using System.Linq;
using Applitools;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Eyes.Selenium.E2ETests
{
    public class ExtractTextTests
    {
        private EyesRunner runner;

        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");
        public static readonly BatchInfo BatchInfo = new BatchInfo("DotNet Multithreads Test");
        private Applitools.Selenium.Eyes eyes;
        private IWebDriver driver;

        [SetUp]
        public void Setup()
        {
            runner = new ClassicRunner();
            eyes = new Applitools.Selenium.Eyes(runner);
            // Use Chrome browser
            var CI = Environment.GetEnvironmentVariable("CI");
            var options = new ChromeOptions();
            //  if (CI != null)
            {
                options.AddArguments("headless");
                options.AddArguments("no-sandbox");
            }
            //Initialize the Runner for your test with concurrency of 5.
            // Create Eyes object with the runner, meaning it'll be a Visual Grid eyes.
            driver = new ChromeDriver(DRIVER_PATH, options); eyes.Batch = BatchInfo;
            eyes.SaveNewTests = false;
            eyes.BranchName = "master";
            eyes.ParentBranchName = "master";
            eyes.HideScrollbars = true;
            var config = eyes.GetConfiguration();
            config.AddBrowsers(new IosDeviceInfo(IosDeviceName.iPad_7));
            config.SetLayoutBreakpoints(500, 1000);
            eyes.SetConfiguration(config);

            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/OCRPage");
        }

        [Test]
        public void ShouldExtractTextFromRegions()
        {
            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/OCRPage");
            driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextFromRegions", new RectangleSize(700, 460));
            var element = driver.FindElement(By.CssSelector("body > h2"));
            var texts = eyes.ExtractText(
                new Applitools.Selenium.OcrRegion(By.CssSelector("body > h1")),
                new Applitools.Selenium.OcrRegion(element),
                new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22))
                    .Hint("imagination be your guide"));
            eyes.Close(false);
            Assert.AreEqual(texts.ElementAt(0), "Header 1: Hello world!");
            Assert.AreEqual(texts.ElementAt(1), "Header 2: He110 world!");
            Assert.AreEqual(texts.ElementAt(2), "imagination be your guide.");
        }

        [Test]
        public void ShouldExtractTextFromRegionsWithoutAHint()
        {
            driver.Navigate().GoToUrl("https://applitools.github.io/demo/TestPages/OCRPage");
            driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextFromRegionsWithoutAHint", new RectangleSize(700, 460));
            var text = eyes.ExtractText(new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22)),
                new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22)));
            eyes.Close(false);
            Assert.AreEqual(text.ElementAt(0), "imagination be your guide.");
            Assert.AreEqual(text.ElementAt(1), "imagination be your guide.");
        }

        //[Test]
        //public void ShouldExtractTextRegionsFromImage()
        //{
        //    driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextRegionsFromImage", new RectangleSize(700, 460));
        //    var regions = eyes.ExtractTextRegions(new TextRegionSettings("header \\d: Hello world", "\\d\\..+", "make")
        //        .SetIgnoreCase(true).SetLanguage("eng"));
        //    eyes.Close(false);
        //    Assert.AreEqual(regions["header \\d: Hello world"].Count, 3);
        //    Assert.AreEqual(regions["header \\d: Hello world"].ElementAt(0).Text, "Header 1: Hello world!");
        //    Assert.AreEqual(regions["header \\d: Hello world"].ElementAt(1).Text, "Header 2: Hello world!");
        //    Assert.AreEqual(regions["header \\d: Hello world"].ElementAt(2).Text, "Header 3: Hello world!");
        //    Assert.AreEqual(regions["\\d\\..+"].Count, 4);
        //    Assert.AreEqual(regions["\\d\\..+"].ElementAt(0).Text, "1.One");
        //    Assert.AreEqual(regions["\\d\\..+"].ElementAt(1).Text, "2.Two");
        //    Assert.AreEqual(regions["\\d\\..+"].ElementAt(2).Text, "3.Three");
        //    Assert.AreEqual(regions["\\d\\..+"].ElementAt(3).Text, "4.Four");
        //}
    }
}