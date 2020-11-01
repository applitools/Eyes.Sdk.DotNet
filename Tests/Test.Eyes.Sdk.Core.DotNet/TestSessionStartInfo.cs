using Applitools.Common;
using Applitools.Fluent;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using NUnit.Framework;
using System;

namespace Applitools
{
    [TestFixture]
    public class TestSessionStartInfo
    {
        [Test]
        public void TestSerialization()
        {
            PropertiesCollection sessionProperties = new PropertiesCollection
            {
                { "property 1", "value 1" },
                { null, null }
            };

            BatchInfo batchInfo = new BatchInfo("some batch", new DateTimeOffset(new DateTime(2017, 6, 29, 11, 1, 0, DateTimeKind.Utc)));
            batchInfo.Id = "someBatchId";

            SessionStartInfo sessionStartInfo = new SessionStartInfo(
                "agent",
                "some app",
                "1.0",
                "some test",
                batchInfo,
                "baseline",
                new AppEnvironment("windows", "test suite", new RectangleSize(234, 456)) { DeviceInfo = "Some Mobile Device" },
                "some environment",
                new ImageMatchSettings(MatchLevel.Strict)
                {
                    Accessibility = new AccessibilityRegionByRectangle[] { new AccessibilityRegionByRectangle(10, 20, 30, 40, AccessibilityRegionType.GraphicalObject) },
                    AccessibilitySettings = new AccessibilitySettings(AccessibilityLevel.AA, AccessibilityGuidelinesVersion.WCAG_2_1),
                    Floating = new FloatingMatchSettings[] { new FloatingMatchSettings(22, 32, 42, 52, 5, 10, 15, 20) }
                },
                "some branch",
                "parent branch",
                "baseline branch",
                saveDiffs: null,
                render: null,
                timeout: 10,
                properties: sessionProperties);

            string json = JsonConvert.SerializeObject(sessionStartInfo, Formatting.Indented);
            string expectedJson = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.SessionStartInfo_Serialization.json");
            Assert.AreEqual(expectedJson, json);
        }

        [TestCaseSource(nameof(ThreeBooleans))]
        public void TestFluentApiSerialization(bool useDom, bool enablePatterns, bool ignoreDisplacements)
        {
            ICheckSettings settings = Target.Window().Fully().UseDom(useDom).EnablePatterns(enablePatterns).IgnoreDisplacements(ignoreDisplacements);
            EyesBase eyes = new TestEyes();
            EyesScreenshot screenshot = new TestEyesScreenshot();
            ImageMatchSettings imageMatchSettings = MatchWindowTask.CreateImageMatchSettings((ICheckSettingsInternal)settings, eyes, screenshot);

            string json = JsonConvert.SerializeObject(imageMatchSettings, Formatting.Indented);
            string expectedJson = CommonUtils.ReadResourceFile($"Test.Eyes.Sdk.Core.DotNet.Resources.SessionStartInfo_FluentApiSerialization_{useDom}_{enablePatterns}_{ignoreDisplacements}.json");
            Assert.AreEqual(expectedJson, json);
        }

        [TestCaseSource(nameof(ThreeBooleans))]
        public void TestImageMatchSettingsSerialization(bool useDom, bool enablePatterns, bool ignoreDisplacements)
        {
            ICheckSettings settings = Target.Window().Fully().UseDom(useDom).EnablePatterns(enablePatterns).IgnoreDisplacements(ignoreDisplacements);
            EyesBase eyes = new TestEyes();
            eyes.DefaultMatchSettings = new ImageMatchSettings(MatchLevel.Exact, new ExactMatchSettings() { MatchThreshold = 0.5 });
            ImageMatchSettings imageMatchSettings = MatchWindowTask.CreateImageMatchSettings(settings as ICheckSettingsInternal, eyes);

            string json = JsonConvert.SerializeObject(imageMatchSettings, Formatting.Indented);
            string expectedJson = CommonUtils.ReadResourceFile($"Test.Eyes.Sdk.Core.DotNet.Resources.SessionStartInfo_FluentApiSerialization_NonDefaultIMS_{useDom}_{enablePatterns}_{ignoreDisplacements}.json");
            Assert.AreEqual(expectedJson, json);
        }

        [TestCaseSource(nameof(ThreeBooleans))]
        public void TestImageMatchSettingsSerialization_Global(bool useDom, bool enablePatterns, bool ignoreDisplacements)
        {
            ICheckSettings settings = Target.Window().Fully().UseDom(useDom).EnablePatterns(enablePatterns);
            TestEyes eyes = new TestEyes();
            IConfiguration configuration = eyes.GetConfiguration();
            configuration.SetDefaultMatchSettings(new ImageMatchSettings(MatchLevel.Exact, new ExactMatchSettings() { MatchThreshold = 0.5 }));
            configuration.SetIgnoreDisplacements(ignoreDisplacements);
            eyes.SetConfiguration(configuration);
            ImageMatchSettings imageMatchSettings = MatchWindowTask.CreateImageMatchSettings(settings as ICheckSettingsInternal, eyes);

            string json = JsonConvert.SerializeObject(imageMatchSettings, Formatting.Indented);
            string expectedJson = CommonUtils.ReadResourceFile($"Test.Eyes.Sdk.Core.DotNet.Resources.SessionStartInfo_FluentApiSerialization_NonDefaultIMS_{useDom}_{enablePatterns}_{ignoreDisplacements}.json");
            Assert.AreEqual(expectedJson, json);
        }


        [TestCaseSource(nameof(ThreeBooleans))]
        public void TestConfigurationSerialization(bool useDom, bool enablePatterns, bool ignoreDisplacements)
        {
            ICheckSettings settings = Target.Window().Fully();
            TestEyes eyes = new TestEyes();
            IConfiguration configuration = eyes.GetConfiguration();
            configuration.SetUseDom(useDom);
            configuration.SetEnablePatterns(enablePatterns);
            configuration.SetIgnoreDisplacements(ignoreDisplacements);
            eyes.SetConfiguration(configuration);

            EyesScreenshot screenshot = new TestEyesScreenshot();
            ImageMatchSettings imageMatchSettings = MatchWindowTask.CreateImageMatchSettings((ICheckSettingsInternal)settings, eyes, screenshot);

            string json = JsonConvert.SerializeObject(imageMatchSettings, Formatting.Indented);
            string expectedJson = CommonUtils.ReadResourceFile($"Test.Eyes.Sdk.Core.DotNet.Resources.SessionStartInfo_FluentApiSerialization_{useDom}_{enablePatterns}_{ignoreDisplacements}.json");
            Assert.AreEqual(expectedJson, json);
        }

        private static object[] ThreeBooleans()
        {
            return new bool[][]
            {
                new bool[] {true, true, true },
                new bool[] {true, true, false },
                new bool[] {true, false, true},
                new bool[] {true, false, false },
                new bool[] {false, true, true },
                new bool[] {false, true, false },
                new bool[] {false, false, true},
                new bool[] {false, false, false },
            };
        }
    }
}
