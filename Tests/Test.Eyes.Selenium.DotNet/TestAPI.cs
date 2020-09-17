using Applitools.Metadata;
using Applitools.Selenium.Fluent;
using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class TestAPI : ReportingTestSuite
    {
        [Test]
        public void EnsureFluentApiCoversEverything()
        {
            Type coreCheckSettings = typeof(CheckSettings);
            Type seleniumCheckSettings = typeof(SeleniumCheckSettings);
            MethodInfo[] mis = coreCheckSettings.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            List<MethodInfo> missingMethods = new List<MethodInfo>();
            List<MethodInfo> methodsReturningWrongType = new List<MethodInfo>();
            List<MethodInfo> methodsNotMarkedWithNew = new List<MethodInfo>();
            foreach (MethodInfo mi in mis)
            {
                if (mi.ReturnType == typeof(ICheckSettings))
                {
                    ParameterInfo[] pis = mi.GetParameters();
                    List<Type> parameters = new List<Type>();
                    foreach (ParameterInfo pi in pis)
                    {
                        parameters.Add(pi.ParameterType);
                    }
                    MethodInfo smi = seleniumCheckSettings.GetMethod(mi.Name, 
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, 
                        null, parameters.ToArray(), null);
                    if (smi == null)
                    {
                        missingMethods.Add(mi);
                        continue;
                    }
                    if (smi.ReturnType != seleniumCheckSettings)
                    {
                        methodsReturningWrongType.Add(smi);
                    }

                    // TODO - find the correct way to figure out if the method was marked with the 'new' keyword
                    //MethodAttributes mAttrs = smi.Attributes;
                    //if (!mAttrs.HasFlag(MethodAttributes.NewSlot))
                    //{
                    //    methodsNotMarkedWithNew.Add(smi);
                    //}
                }
            }

            StringBuilder sb = new StringBuilder();

            WriteMethodsToStringBuilder(sb,
                $"methods in {nameof(CheckSettings)} that are not overriden in {nameof(SeleniumCheckSettings)}:",
                missingMethods);

            WriteMethodsToStringBuilder(sb,
                $"methods in {nameof(SeleniumCheckSettings)} that does not return {nameof(SeleniumCheckSettings)}:",
                methodsReturningWrongType);

            WriteMethodsToStringBuilder(sb,
                $"methods in {nameof(SeleniumCheckSettings)} that are not marked with 'new':",
                methodsNotMarkedWithNew);

            if (sb.Length > 0)
            {
                Assert.Fail(sb.ToString());
            }
        }

        private static void WriteMethodsToStringBuilder(StringBuilder sb, string title, List<MethodInfo> methods)
        {
            if (methods.Count > 0)
            {
                sb.AppendLine(title);
                foreach (MethodInfo mi in methods)
                {
                    sb.Append("\t");
                    AppendMethodInfoToStringBuilder(sb, mi);
                    sb.AppendLine();
                }
            }
        }

        public static void AppendMethodInfoToStringBuilder(StringBuilder sb, MethodInfo mi)
        {
            sb.Append(mi.Name);
            ParameterInfo[] pis = mi.GetParameters();
            if (pis != null && pis.Length > 0)
            {
                sb.Append("(");
                foreach (ParameterInfo pi in pis)
                {
                    sb.Append(pi.ParameterType).Append(" ").Append(pi.Name).Append(", ");
                }
                sb.Length -= 2;
                sb.Append(")");
            }
        }

        [Test]
        public void TestIsDisabled()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = new Eyes();
            eyes.Batch = TestDataProvider.BatchInfo;
            TestUtils.SetupLogging(eyes);
            eyes.IsDisabled = true;
            try
            {
                eyes.Open(driver, "Demo C# app", "hello world", new Size(800, 600));

                driver.Url = "https://applitools.com/helloworld?diff1";
                eyes.Check(Target.Window().Fully());
                eyes.Check("Number test", Target.Region(By.ClassName("random-number")));
                eyes.Check(Target.Window().WithName("1"), Target.Region(By.Id("SomeId")).WithName("2"));
                eyes.CheckFrame(new string[] { "a", "b" }, TimeSpan.FromSeconds(1));

                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestAccessibility(bool useVisualGrid)
        {
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            string suffix = useVisualGrid ? "_VG" : "";
            Eyes eyes = new Eyes(runner);
            eyes.Batch = TestDataProvider.BatchInfo;
            TestUtils.SetupLogging(eyes, nameof(TestAccessibility) + suffix);
            AccessibilitySettings settings = new AccessibilitySettings(AccessibilityLevel.AA, AccessibilityGuidelinesVersion.WCAG_2_0);
            Configuration configuration = eyes.GetConfiguration();
            configuration.SetAccessibilityValidation(settings);
            eyes.SetConfiguration(configuration);
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            try
            {
                driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
                eyes.Open(driver, "Applitools Eyes SDK", "TestAccessibility_Sanity" + suffix, new Size(700, 460));
                eyes.Check("Sanity", Target.Window().Accessibility(By.ClassName("ignore"), AccessibilityRegionType.LargeText));
                eyes.CloseAsync();
                configuration.SetAccessibilityValidation(null);
                eyes.SetConfiguration(configuration);
                eyes.Open(driver, "Applitools Eyes SDK", "TestAccessibility_No_Accessibility" + suffix, new Size(1200, 800));
                eyes.CheckWindow("No accessibility");
                eyes.CloseAsync();
            }
            finally
            {
                driver.Quit();
                eyes.AbortAsync();
                TestResultsSummary allTestResults = runner.GetAllTestResults(false);

                Assert.AreEqual(2, allTestResults.Count);
                TestResults resultSanity = allTestResults[0].TestResults;
                TestResults resultNoAccessibility = allTestResults[1].TestResults;
                // Visual grid runner doesn't guarantee the order of the results
                if (resultNoAccessibility.Name.StartsWith("TestAccessibility_Sanity"))
                {
                    TestResults temp = resultSanity;
                    resultSanity = resultNoAccessibility;
                    resultNoAccessibility = temp;
                }

                // Testing the accessibility status returned in the results
                SessionAccessibilityStatus accessibilityStatus = resultSanity.AccessibilityStatus;
                Assert.AreEqual(AccessibilityLevel.AA, accessibilityStatus.Level);
                Assert.AreEqual(AccessibilityGuidelinesVersion.WCAG_2_0, accessibilityStatus.Version);

                Assert.IsNull(resultNoAccessibility.AccessibilityStatus);

                // Testing the accessibility settings sent in the start info
                SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, resultSanity);
                Metadata.ImageMatchSettings defaultMatchSettings = sessionResults.StartInfo.DefaultMatchSettings;
                Assert.AreEqual(AccessibilityGuidelinesVersion.WCAG_2_0, defaultMatchSettings.AccessibilitySettings.GuidelinesVersion);
                Assert.AreEqual(AccessibilityLevel.AA, defaultMatchSettings.AccessibilitySettings.Level);

                // Testing the accessibility regions sent in the session
                var matchSettings = sessionResults.ActualAppOutput[0].ImageMatchSettings;
                var actualRegions = matchSettings.Accessibility;
                HashSet<AccessibilityRegionByRectangle> expectedRegions = new HashSet<AccessibilityRegionByRectangle>();
                expectedRegions.Add(new AccessibilityRegionByRectangle(122, 928, 456, 306, AccessibilityRegionType.LargeText));
                expectedRegions.Add(new AccessibilityRegionByRectangle(8, 1270, 690, 206, AccessibilityRegionType.LargeText));
                if (useVisualGrid)
                {
                    expectedRegions.Add(new AccessibilityRegionByRectangle(10, 284, 800, 500, AccessibilityRegionType.LargeText));
                }
                else
                {
                    expectedRegions.Add(new AccessibilityRegionByRectangle(10, 284, 285, 165, AccessibilityRegionType.LargeText));
                }
                TestSetup.CompareAccessibilityRegionsList_(actualRegions, expectedRegions, "Accessibility");
            }
        }

        [Test]
        public void TestReplaceMatchedStep()
        {
            Eyes eyes = new Eyes(new MockServerConnectorFactory());
            TestUtils.SetupLogging(eyes);
            eyes.Batch = TestDataProvider.BatchInfo;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            MockServerConnector mockServerConnector = (MockServerConnector)eyes.seleniumEyes_.ServerConnector;
            mockServerConnector.AsExcepted = false;
            try
            {
                driver.Url = "https://applitools.github.io/demo/TestPages/SpecialCases/everchanging.html";
                eyes.Open(driver, "Applitools Eyes SDK", "Test Replace Matched Step", new Size(700, 460));
                eyes.Check("Step 1", Target.Window());
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
            Assert.AreEqual(1, mockServerConnector.MatchWindowCalls.Count, "Matches");
            Assert.That(mockServerConnector.ReplaceMatchedStepCalls.Count, Is.GreaterThan(0), "Replacements");
        }


        [Test]
        public void TestNoReplaceMatchedStep()
        {
            Eyes eyes = new Eyes(new MockServerConnectorFactory());
            TestUtils.SetupLogging(eyes);
            eyes.Batch = TestDataProvider.BatchInfo;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            MockServerConnector mockServerConnector = (MockServerConnector)eyes.seleniumEyes_.ServerConnector;
            mockServerConnector.AsExcepted = false;
            try
            {
                driver.Url = "https://applitools.github.io/demo/TestPages/SpecialCases/neverchanging.html";
                eyes.Open(driver, "Applitools Eyes SDK", "Test No Replace Matched Step", new Size(700, 460));
                eyes.Check("Step 1", Target.Window());
                eyes.Close();
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
            }
            Assert.AreEqual(1, mockServerConnector.MatchWindowCalls.Count, "Matches");
            Assert.AreEqual(0, mockServerConnector.ReplaceMatchedStepCalls.Count, "Replacements");
        }

        [Test]
        public void TestEyesConstructor_PassServerUrl()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Uri serverUrl = new Uri(serverUrlStr);
            Eyes eyes = new Eyes(serverUrl);
            Assert.AreEqual(serverUrl.ToString(), eyes.ServerUrl);
        }

        [Test]
        public void TestFullAgentId_1()
        {
            Eyes eyes = new Eyes();
            StringAssert.StartsWith("Eyes.Selenium.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_2()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Uri serverUrl = new Uri(serverUrlStr);
            Eyes eyes = new Eyes(serverUrl);
            StringAssert.StartsWith("Eyes.Selenium.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_3()
        {
            Eyes eyes = new Eyes(new ServerConnectorFactory());
            StringAssert.StartsWith("Eyes.Selenium.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_VG()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            StringAssert.StartsWith("Eyes.Selenium.VisualGrid.DotNet/", eyes.FullAgentId);
        }
    }
}
