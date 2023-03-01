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
			driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextFromRegions", new RectangleSize(700, 460));
			var element = driver.FindElement(By.CssSelector("body > h2"));
			var texts = eyes.ExtractText(
				new Applitools.Selenium.OcrRegion(By.CssSelector("body > h1")),
				new Applitools.Selenium.OcrRegion(element),
				new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22))
					.Hint("imagination be your guide"));
			eyes.Close(false);
			Assert.AreEqual("Header 1: Hello world!", texts.ElementAt(0));
			Assert.AreEqual("Header 2: He110 world!", texts.ElementAt(1));
			Assert.AreEqual("imagination be your guide.", texts.ElementAt(2));
		}

		[Test]
		public void ShouldExtractTextFromRegionsWithoutAHint()
		{
			driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextFromRegionsWithoutAHint", new RectangleSize(700, 460));
			var text = eyes.ExtractText(new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22)),
				new Applitools.Selenium.OcrRegion(new Region(10, 405, 210, 22)));
			eyes.Close(false);
			Assert.AreEqual("imagination be your guide.", text.ElementAt(0));
			Assert.AreEqual("imagination be your guide.", text.ElementAt(1));
		}

		[Test]
		public void ShouldExtractTextRegionsFromImage()
		{
			driver = eyes.Open(driver, "Applitools Eyes SDK", "ShouldExtractTextRegionsFromImage", new RectangleSize(700, 460));
			var regions = eyes.ExtractTextRegions(new TextRegionSettings("header \\d: Hello world", "\\d\\..+", "make")
				.SetIgnoreCase(true));
			eyes.Close(false);
			Assert.AreEqual(3, regions["header \\d: Hello world"].Count);
			Assert.AreEqual("Header 1: Hello world!", regions["header \\d: Hello world"].ElementAt(0).Text);
			Assert.AreEqual("Header 2: Hello world!", regions["header \\d: Hello world"].ElementAt(1).Text);
			Assert.AreEqual("Header 3: Hello world!", regions["header \\d: Hello world"].ElementAt(2).Text);
			Assert.AreEqual(regions["\\d\\..+"].Count, 4);
			Assert.AreEqual("1.One", regions["\\d\\..+"].ElementAt(0).Text);
			Assert.AreEqual("2.Two", regions["\\d\\..+"].ElementAt(1).Text);
			Assert.AreEqual("3.Three", regions["\\d\\..+"].ElementAt(2).Text);
			Assert.AreEqual("4.Four", regions["\\d\\..+"].ElementAt(3).Text);
		}
	}
}