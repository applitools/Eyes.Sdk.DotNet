using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestFluentApi_Frames : TestSetup
    {
        public TestFluentApi_Frames(string testSuitName) : base(testSuitName) { }

        public TestFluentApi_Frames(DriverOptions options) : this(options, false) { }

        public TestFluentApi_Frames(DriverOptions options, bool useVisualGrid)
            : base("Eyes Selenium SDK - Fluent API", options, useVisualGrid) { }

        public TestFluentApi_Frames(DriverOptions options, StitchModes stitchMode)
            : base("Eyes Selenium SDK - Fluent API", options, stitchMode) { }

        [Test]
        public void TestCheckFrame_Fully_Fluent()
        {
            GetEyes().Check("Fluent - Full Frame", Target.Frame("frame1").Fully());
        }

        [Test]
        public void TestCheckFrame_Fluent()
        {
            GetEyes().Check("Fluent - Frame", Target.Frame("frame1"));
        }

        [Test]
        public void TestCheckFrame_Viewport_Fluent()
        {
            IWebElement frame1 = GetWebDriver().FindElement(By.Name("frame1"));
            GetWebDriver().ExecuteJavaScript("arguments[0].style.borderWidth='3px'", frame1);
            GetEyes().Check("Fluent - Frame - Viewport with ignore region",
                Target.Frame("frame1").Fully(false).Ignore(By.CssSelector("#inner-frame-div")));
            SetExpectedIgnoreRegions(new Region(11, 11, 304, 184));
        }

        [Test]
        public void TestCheckFrameInFrame_Fully_Fluent()
        {
            GetEyes().Check("Fluent - Full Frame in Frame", Target.Frame("frame1")
                                                     .Frame("frame1-1")
                                                     .Fully());
        }

        /*
        [Test, Ignore("Code doesn't work yet")]
        public void TestCheckFrameInFrame_Fluent()
        {
            eyes_.Check("Fluent - Frame in Frame", Target.Frame("frame1")
                                                .Frame("frame1-1"));
        }
        */

        [Test]
        public void TestCheckRegionInFrame_Fluent()
        {
            GetEyes().Check("Fluent - Region in Frame", Target.Frame("frame1")
                                                 .Region(By.Id("inner-frame-div"))
                                                 .Fully());
        }

        [Test]
        public void TestCheckRegionInFrameInFrame_Fluent()
        {
            GetEyes().Check("Fluent - Region in Frame in Frame", Target.Frame("frame1")
                                                          .Frame("frame1-1")
                                                          .Region(By.TagName("img"))
                                                          .Fully());
        }

        [Test]
        public void TestCheckRegionInFrame2_Fluent()
        {
            GetEyes().Check("Fluent - Inner frame div 1", Target.Frame("frame1")
                                                   .Region(By.Id("inner-frame-div"))
                                                   .Fully()
                                                   .Timeout(TimeSpan.FromSeconds(5))
                                                   .Ignore(new Rectangle(50, 50, 100, 100)));

            GetEyes().Check("Fluent - Inner frame div 2", Target.Frame("frame1")
                                                   .Region(By.Id("inner-frame-div"))
                                                   .Fully()
                                                   .Ignore(new Rectangle(50, 50, 100, 100))
                                                   .Ignore(new Rectangle(70, 170, 90, 90)));

            GetEyes().Check("Fluent - Inner frame div 3", Target.Frame("frame1")
                                                   .Region(By.Id("inner-frame-div"))
                                                   .Fully()
                                                   .Timeout(TimeSpan.FromSeconds(5)));

            GetEyes().Check("Fluent - Inner frame div 4", Target.Frame("frame1")
                                                   .Region(By.Id("inner-frame-div"))
                                                   .Fully());

            GetEyes().Check("Fluent - Full frame with floating region", Target.Frame("frame1")
                                                                 .Fully()
                                                                 .Layout()
                                                                 .Floating(25, new Rectangle(200, 200, 150, 150)));
        }

        [Test]
        public void TestCheckRegionInFrame3_Fluent()
        {
            GetEyes().Check("Fluent - Full frame with floating region", Target.Frame("frame1")
                                                             .Fully()
                                                             .Layout()
                                                             .Floating(25, new Rectangle(200, 200, 150, 150)));

            SetExpectedFloatingRegions(new FloatingMatchSettings(200, 200, 150, 150, 25, 25, 25, 25));
        }

        [Test]
        public void TestCheckRegionByCoordinateInFrameFully_Fluent()
        {
            GetEyes().Check("Fluent - Inner frame coordinates", Target.Frame("frame1")
                                                         .Region(new Rectangle(30, 40, 400, 1200))
                                                         .Fully());
        }

        [Test]
        public void TestCheckRegionByCoordinateInFrame_Fluent()
        {
            GetEyes().Check("Fluent - Inner frame coordinates", Target.Frame("frame1")
                                                         .Region(new Rectangle(30, 40, 400, 1200)));
        }

        [Test]
        public void TestCheckFrameInFrame_Fully_Fluent2()
        {
            GetEyes().Check("Fluent - Window with Ignore region 2", Target.Window()
                    .Fully());

            GetEyes().Check("Fluent - Full Frame in Frame 2", Target.Frame("frame1")
                    .Frame("frame1-1")
                    .Fully());
        }

        [Test]
        public void TestCheckLongIFrameModal()
        {
            GetWebDriver().FindElement(By.Id("stretched")).Click();
            IWebElement frame = GetDriver().FindElement(By.CssSelector("#modal2 iframe"));
            GetDriver().SwitchTo().Frame(frame);
            IWebElement element = GetDriver().FindElement(By.TagName("html"));
            Size size = element.Size;
            Point location = element.Location;
            Rectangle elementRect = new Rectangle(location, size);
            Rectangle rect;
            List<ICheckSettings> targets = new List<ICheckSettings>();
            for (int i = location.Y, c = 1; i < location.Y + size.Height; i += 5000, c++)
            {
                if (elementRect.Bottom > i + 5000)
                {
                    rect = new Rectangle(location.X, i, size.Width, 5000);
                }
                else
                {
                    rect = new Rectangle(location.X, i, size.Width, elementRect.Bottom - i);
                }
                targets.Add(Target.Region(rect));
                //eyes_.Check("Long IFrame Modal #" + c, Target.Region(rect).Fully());
            }
            GetEyes().Check(targets.ToArray());
        }

        [Test]
        public void TestCheckLongOutOfBoundsIFrameModal()
        {
            GetWebDriver().FindElement(By.Id("hidden_click")).Click();
            IWebElement frame = GetDriver().FindElement(By.CssSelector("#modal3 iframe"));
            GetDriver().SwitchTo().Frame(frame);
            IWebElement element = GetDriver().FindElement(By.TagName("html"));
            Size size = element.Size;
            Point location = element.Location;
            Rectangle elementRect = new Rectangle(location, size);
            Rectangle rect;
            List<ICheckSettings> targets = new List<ICheckSettings>();
            for (int i = location.Y, c = 1; i < location.Y + size.Height; i += 5000, c++)
            {
                if (elementRect.Bottom > i + 5000)
                {
                    rect = new Rectangle(location.X, i, size.Width, 5000);
                }
                else
                {
                    rect = new Rectangle(location.X, i, size.Width, elementRect.Bottom - i);
                }
                targets.Add(Target.Region(rect));
                //eyes_.Check("Long IFrame Modal #" + c, Target.Region(rect).Fully());
            }
            GetEyes().Check(targets.ToArray());
        }
    }
}
