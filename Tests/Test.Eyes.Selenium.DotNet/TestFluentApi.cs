using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;
using ExpectedConditions = SeleniumExtras.WaitHelpers.ExpectedConditions;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestFluentApi : TestSetup
    {
        public TestFluentApi(string testSuitName) : base(testSuitName) { }

        public TestFluentApi(DriverOptions options) : this(options, false) { }

        public TestFluentApi(DriverOptions options, bool useVisualGrid)
         : base("Eyes Selenium SDK - Fluent API", options, useVisualGrid) { }

        public TestFluentApi(DriverOptions options, StitchModes stitchMode)
         : base("Eyes Selenium SDK - Fluent API", options, stitchMode) { }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckWindowWithIgnoreRegion_Fluent()
        {
            GetDriver().FindElement(By.TagName("input")).SendKeys("My Input");
            GetEyes().Check("Fluent - Window with Ignore region", Target.Window()
                                                           .Fully()
                                                           .Timeout(TimeSpan.FromSeconds(5))
                                                           .IgnoreCaret()
                                                           .Ignore(new Rectangle(50, 50, 100, 100)));

            SetExpectedIgnoreRegions(new Region(50, 50, 100, 100));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckRegionWithIgnoreRegion_Fluent()
        {
            GetEyes().Check("Fluent - Region with Ignore region", Target.Region(By.Id("overflowing-div"))
                                                           .Ignore(new Rectangle(50, 50, 100, 100)));

            SetExpectedIgnoreRegions(new Region(50, 50, 100, 100));
        }

        [Test]
        public void TestCheckRegionBySelectorAfterManualScroll_Fluent()
        {
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("window.scrollBy(0,900)");
            GetEyes().Check("Fluent - Region by selector after manual scroll", Target.Region(By.Id("centered")));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckWindow_Fluent()
        {
            GetEyes().Check("Fluent - Window", Target.Window());
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckWindowWithIgnoreBySelector_Fluent()
        {
            GetEyes().Check("Fluent - Window with ignore region by selector", Target.Window()
                    .Ignore(By.Id("overflowing-div")));

            SetExpectedIgnoreRegions(new Region(8, 81, 304, 184));
        }

        [Test]
        public void TestCheckWindowWithIgnoreBySelector_Centered_Fluent()
        {
            GetEyes().Check("Fluent - Window with ignore region by selector centered", Target.Window()
                    .Ignore(By.Id("centered")));

            SetExpectedIgnoreRegions(new Region(122, 933, 456, 306));
        }

        [Test]
        public void TestCheckWindowWithIgnoreBySelector_Stretched_Fluent()
        {
            GetEyes().Check("Fluent - Window with ignore region by selector stretched", Target.Window()
                    .Ignore(By.Id("stretched")));

            SetExpectedIgnoreRegions(new Region(8, 1277, 690, 206));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckWindowWithFloatingBySelector_Fluent()
        {
            GetEyes().Check("Fluent - Window with ignore region by selector", Target.Window()
                    .Floating(By.Id("overflowing-div"), 3, 3, 20, 30));

            SetExpectedFloatingRegions(new FloatingMatchSettings(8, 81, 304, 184, 3, 3, 20, 30));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckRegionByCoordinates_Fluent()
        {
            GetEyes().Check("Fluent - Region by coordinates", Target.Region(new Rectangle(50, 70, 90, 110)));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckOverflowingRegionByCoordinates_Fluent()
        {
            GetEyes().Check("Fluent - Region by overflowing coordinates", Target.Region(new Rectangle(50, 110, 90, 550)));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckElementWithIgnoreRegionByElementOutsideTheViewport_Fluent()
        {
            IWebElement element = GetWebDriver().FindElement(By.Id("overflowing-div-image"));
            IWebElement ignoreElement = GetDriver().FindElement(By.Id("overflowing-div"));
            GetEyes().Check("Fluent - Region by element", Target.Region(element).Ignore(ignoreElement));
            SetExpectedIgnoreRegions(new Region(0, -203, 304, 184));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckElementWithIgnoreRegionBySameElement_Fluent()
        {
            IWebElement element = GetWebDriver().FindElement(By.Id("overflowing-div-image"));
            GetEyes().Check("Fluent - Region by element", Target.Region(element).Ignore(element));
            SetExpectedIgnoreRegions(new Region(0, 0, 304, 184));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestScrollbarsHiddenAndReturned_Fluent()
        {
            //eyes_.SendDom = false;
            GetEyes().Check("Fluent - Window (Before)", Target.Window().Fully());
            GetEyes().Check("Fluent - Inner frame div",
                    Target.Frame("frame1")
                            .Region(By.Id("inner-frame-div"))
                            .Fully());
            GetEyes().Check("Fluent - Window (After)", Target.Window().Fully());
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckFullWindowWithMultipleIgnoreRegionsBySelector_Fluent()
        {
            GetEyes().Check("Fluent - Region by element", Target.Window().Fully().Ignore(By.CssSelector(".ignore")));
            if (useVisualGrid_)
            {
                SetExpectedIgnoreRegions(
                    new Region(122, 933, 456, 306),
                    new Region(8, 1277, 690, 206),
                    new Region(10, 286, 800, 500)
                );
            }
            else
            {
                SetExpectedIgnoreRegions(
                    new Region(122, 933, 456, 306),
                    new Region(8, 1277, 690, 206),
                    new Region(10, 286, 285, 165)
                );
            }
        }


        [Test]
        public void TestCheckMany()
        {
            WebDriverWait wait = new WebDriverWait(GetDriver(), TimeSpan.FromSeconds(30));
            GetDriver().SwitchTo().Frame("frame1");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("iframe[name=frame1-1]")));
            GetDriver().SwitchTo().Frame("frame1-1");
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("img")));
            GetDriver().SwitchTo().DefaultContent();
            GetEyes().Check(
                    Target.Region(By.Id("overflowing-div-image")).WithName("overflowing div image"),
                    Target.Region(By.Id("overflowing-div")).WithName("overflowing div"),
                    Target.Region(By.Id("overflowing-div-image")).Fully().WithName("overflowing div image (fully)"),
                    Target.Frame("frame1").Frame("frame1-1").Fully().WithName("Full Frame in Frame"),
                    Target.Frame("frame1").WithName("frame1"),
                    Target.Region(new Rectangle(30, 50, 300, 620)).WithName("rectangle")
            );
        }

        [Test]
        public void TestCheckScrollableModal()
        {
            GetDriver().FindElement(By.Id("centered")).Click();
            GetEyes().Check("Scrollable Modal", Target.Region(By.Id("modal-content")).Fully().ScrollRootElement(By.Id("modal1")));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestIgnoreDisplacements(bool ignoreDisplacements)
        {
            GetEyes().Check($"Fluent - Ignore Displacements = {ignoreDisplacements}", Target.Window().IgnoreDisplacements(ignoreDisplacements).Fully());
            AddExpectedProperty(nameof(ImageMatchSettings.IgnoreDisplacements), ignoreDisplacements);
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckWindowWithFloatingByRegion_Fluent()
        {
            ICheckSettings settings = Target.Window()
                    .Floating(new Rectangle(10, 10, 20, 20), 3, 3, 20, 30);
            GetEyes().Check("Fluent - Window with floating region by region", settings);

            SetExpectedFloatingRegions(new FloatingMatchSettings(10, 10, 20, 20, 3, 3, 20, 30));
        }

        [Test]
        public void TestCheckElementFully_Fluent()
        {
            IWebElement element = GetWebDriver().FindElement(By.Id("overflowing-div-image"));
            GetEyes().Check("Fluent - Region by element - fully", Target.Region(element).Fully());
        }

        [Test]
        public void TestSimpleRegion()
        {
            GetEyes().Check(Target.Window().Region(new Rectangle(50, 50, 100, 100)));
        }

        [Test]
        public void TestSimpleRegionBiggerThenTheViewport()
        {
            GetEyes().Check(Target.Window().Region(new Rectangle(0, 0, 2000, 2000)));
        }

        [Test]
        [Skip(TestUtils.COVERED_BY_GENERATED_TESTS_MESSAGE)]
        public void TestCheckElementFullyAfterScroll()
        {
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("window.scrollTo(0, 500)");
            IWebElement element = GetWebDriver().FindElement(By.Id("overflowing-div-image"));
            GetEyes().Check("Fluent - Region by element - fully after scroll", Target.Region(element).Fully());
        }

        [Test]
        public void TestAccessibilityRegions()
        {
            IConfiguration config = GetEyes().GetConfiguration();
            config.AccessibilityValidation = new AccessibilitySettings(AccessibilityLevel.AAA, AccessibilityGuidelinesVersion.WCAG_2_0);
            GetEyes().SetConfiguration(config);
            GetEyes().Check(Target.Window().Accessibility(By.ClassName("ignore"), AccessibilityRegionType.LargeText));
            
            if (useVisualGrid_)
            {
                SetExpectedAccessibilityRegions(
                    new AccessibilityRegionByRectangle(122, 933, 456, 306, AccessibilityRegionType.LargeText),
                    new AccessibilityRegionByRectangle(8, 1277, 690, 206, AccessibilityRegionType.LargeText),
                    new AccessibilityRegionByRectangle(10, 286, 800, 500, AccessibilityRegionType.LargeText)
                    );
            }
            else
            {
                SetExpectedAccessibilityRegions(
                    new AccessibilityRegionByRectangle(122, 933, 456, 306, AccessibilityRegionType.LargeText),
                    new AccessibilityRegionByRectangle(8, 1277, 690, 206, AccessibilityRegionType.LargeText),
                    new AccessibilityRegionByRectangle(10, 286, 285, 165, AccessibilityRegionType.LargeText)
                    );
            }

            AddExpectedProperty(nameof(ImageMatchSettings.AccessibilitySettings) + "." + nameof(AccessibilitySettings.Level), AccessibilityLevel.AAA);
            AddExpectedProperty(nameof(ImageMatchSettings.AccessibilitySettings) + "." + nameof(AccessibilitySettings.GuidelinesVersion), AccessibilityGuidelinesVersion.WCAG_2_0);
        }
    }
}
