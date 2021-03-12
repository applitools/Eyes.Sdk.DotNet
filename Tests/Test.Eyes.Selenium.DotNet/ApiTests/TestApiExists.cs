using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestApiExists : ReportingTestSuite
    {
        [Test]
        public void TestApiProperties()
        {
            Eyes eyes = new Eyes();

            eyes.AppName = "app name";
            eyes.TestName = "test name";
            eyes.BaselineBranchName = "baseline branch name";
            eyes.ParentBranchName = "parent branch name";
            eyes.BranchName = "branch name";
            eyes.BaselineEnvName = "baseline env name";
            eyes.EnvironmentName = "env name";
            eyes.HostApp = "host app";
            eyes.HostOS = "windows";
            eyes.AgentId = "some agent id";

            string fullAgentId = eyes.FullAgentId;

            eyes.IgnoreCaret = true;
            eyes.HideCaret = true;
#pragma warning disable CS0618 // Type or member is obsolete
            eyes.SaveFailedTests = true;
#pragma warning restore CS0618 // Type or member is obsolete
            eyes.SaveNewTests = true;
            eyes.SendDom = true;
            eyes.SaveDiffs = true;

            eyes.StitchOverlap = 20;

            eyes.Batch = new BatchInfo();
#pragma warning disable CS0612 // Type or member is obsolete
            eyes.FailureReports = FailureReports.Immediate;
#pragma warning restore CS0612 // Type or member is obsolete
            eyes.MatchTimeout = TimeSpan.FromSeconds(30);

            eyes.ViewportSize = new RectangleSize(1000, 600);
            eyes.ViewportSize = new Size(1000, 600);

            eyes.MatchLevel = MatchLevel.Strict;

            double dpr = eyes.DevicePixelRatio;
            double sr = eyes.ScaleRatio;
            eyes.SetScrollToRegion(true);

            Configuration config = eyes.GetConfiguration();

            Assert.AreEqual(config.AppName, "app name");
            Assert.AreEqual(config.TestName, "test name");
            Assert.AreEqual(config.BaselineBranchName, "baseline branch name");
            Assert.AreEqual(config.ParentBranchName, "parent branch name");
            Assert.AreEqual(config.BranchName, "branch name");
            Assert.AreEqual(config.BaselineEnvName, "baseline env name");
            Assert.AreEqual(config.EnvironmentName, "env name");
            Assert.AreEqual(config.HostApp, "host app");
            Assert.AreEqual(config.HostOS, "windows");
            Assert.AreEqual(config.AgentId, "some agent id");

            Assert.AreEqual(config.IgnoreCaret, true);
            Assert.AreEqual(config.HideCaret, true);
            Assert.AreEqual(config.SaveNewTests, true);
            Assert.AreEqual(config.SendDom, true);
            Assert.AreEqual(config.SaveDiffs, true);

            Assert.AreEqual(config.StitchOverlap, 20);
            Assert.AreSame(config.Batch, eyes.Batch);

            Assert.AreEqual(config.MatchTimeout, TimeSpan.FromSeconds(30));
            Assert.AreEqual(config.ViewportSize.ToString(), new RectangleSize(1000, 600).ToString());
            Assert.AreEqual(config.MatchLevel, MatchLevel.Strict);

            Assert.AreEqual(config.IgnoreDisplacements, false);
            config.IgnoreDisplacements = true;
            foreach (AccessibilityGuidelinesVersion version in Enum.GetValues(typeof(AccessibilityGuidelinesVersion)))
            {
                foreach (AccessibilityLevel level in Enum.GetValues(typeof(AccessibilityLevel)))
                {
                    config.SetAccessibilityValidation(new AccessibilitySettings(level, version));
                    Assert.AreEqual(level, config.AccessibilityValidation.Level);
                    Assert.AreEqual(version, config.AccessibilityValidation.GuidelinesVersion);
                }
            }
            config.SetAccessibilityValidation(null);
            Assert.IsNull(config.AccessibilityValidation);
        }
    }
}
