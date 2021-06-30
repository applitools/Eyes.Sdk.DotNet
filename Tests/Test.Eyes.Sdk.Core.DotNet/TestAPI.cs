using Applitools.Common;
using Applitools.Fluent;
using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using Applitools.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using Applitools.VisualGrid;
using System.Net;
using System.Linq;

namespace Applitools.Tests
{
    [TestFixture]
    public class TestAPI : ReportingTestSuite
    {
        [Test]
        public void EnsureSetMethodPerProperty()
        {
            Type configType = typeof(IConfiguration);
            Type seleniumConfigType = typeof(Selenium.IConfiguration);
            PropertyInfo[] properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            MethodInfo[] methodInfos = configType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<MethodInfo> overloads = new List<MethodInfo>();

            foreach (PropertyInfo pi in properties)
            {
                Assert.NotNull(pi.GetSetMethod(), $"Property {pi.Name} doesn't have SET method");
                Assert.NotNull(pi.GetGetMethod(), $"Property {pi.Name} doesn't have GET method");
                {
                    MethodInfo[] miArr = methodInfos.Where(m => m.Name.Equals("Set" + pi.Name)).ToArray();
                    overloads.AddRange(miArr);
                    if (miArr.Length == 0)
                    {
                        Assert.Fail("property '{0}' doesn't have matching setter", pi.Name);
                    }
                    else if (miArr.Length == 1)
                    {
                        MethodInfo mi = miArr[0];
                        Assert.AreEqual(configType, mi.ReturnType);
                        ParameterInfo[] paramsInfo = mi.GetParameters();
                        Assert.AreEqual(1, paramsInfo.Length);
                        Assert.IsTrue(pi.PropertyType.IsAssignableFrom(paramsInfo[0].ParameterType),
                            "Setter method parameter type {0} is not assignable from {1}", paramsInfo[0].ParameterType, pi.PropertyType);
                        //Assert.AreEqual(pi.PropertyType, paramsInfo[0].ParameterType);
                    }
                    else if (miArr.Length > 1)
                    {
                        bool foundAMatch = false;
                        foreach (MethodInfo mi in miArr)
                        {
                            ParameterInfo[] paramsInfo = mi.GetParameters();
                            Assert.AreEqual(1, paramsInfo.Length);
                            if (pi.PropertyType.IsAssignableFrom(paramsInfo[0].ParameterType))
                            {
                                foundAMatch = true;
                            }
                        }
                        Assert.IsTrue(foundAMatch,
                            "No Setter method overload found for property {0} ({1}). Relevant overloads: {2}", pi.Name, pi.PropertyType,
                            string.Join(", ", (IEnumerable<MethodInfo>)miArr));
                    }
                }
            }

            methodInfos = seleniumConfigType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            methodInfos = methodInfos.Where(mi => mi.GetParameters()?.Length == 1).ToArray();
            foreach (MethodInfo omi in overloads)
            {
                MethodInfo mi = methodInfos.Where(mi => mi.Name == omi.Name &&
                    mi.GetParameters()[0].ParameterType == omi.GetParameters()[0].ParameterType).FirstOrDefault();
                Assert.NotNull(mi, "method 'Set{0}({1})' isn't overriden in Selenium.Configuration", omi.Name, omi.GetParameters()[0].ParameterType);
                MethodAttributes mAttrs = mi.Attributes;
                if (!mAttrs.HasFlag(MethodAttributes.NewSlot))
                {
                    Assert.Fail("overriden method '{0}' is not marked in new", mi.Name);
                }
                Assert.AreEqual(seleniumConfigType, mi.ReturnType);
            }
            TestContext.Progress.WriteLine("{0} properties has set methods.", properties.Length);
            TestContext.Progress.WriteLine("{0} Set methods overriden in {1}.", overloads.Count, seleniumConfigType);
        }

        [Test]
        public void EnsureSeleniumSetMethodPerProperty()
        {
            Type configType = typeof(Selenium.IConfiguration);
            PropertyInfo[] properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                Assert.NotNull(pi.GetSetMethod());
                Assert.NotNull(pi.GetGetMethod());
                {
                    string propertyName = pi.Name;
                    if (propertyName.Length > 2 && propertyName.StartsWith("Is") && char.IsUpper(propertyName[2]))
                    {
                        propertyName = propertyName.Substring(2);
                    }
                    MethodInfo mi = configType.GetMethod("Set" + propertyName, BindingFlags.Public | BindingFlags.Instance);
                    Assert.NotNull(mi, "property '{0}' doesn't have matching setter 'Set{1}'", pi.Name, propertyName);
                    Assert.AreEqual(configType, mi.ReturnType);
                    ParameterInfo[] paramsInfo = mi.GetParameters();
                    Assert.AreEqual(1, paramsInfo.Length);
                    Type underlyingType = Nullable.GetUnderlyingType(pi.PropertyType);
                    if (underlyingType != null)
                    {
                        Assert.AreEqual(underlyingType, paramsInfo[0].ParameterType);
                    }
                    else
                    {
                        Assert.AreEqual(pi.PropertyType, paramsInfo[0].ParameterType);
                    }
                }
            }
            TestContext.Progress.WriteLine("{0} properties has set methods.", properties.Length);
        }

        [Test]
        public void TestConfigApiKey()
        {
            TestEyes testEyes = new TestEyes();
            IConfiguration config = testEyes.GetConfiguration();
            config.ApiKey = "someTestApiKey";
            testEyes.SetConfiguration(config);
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            IServerConnector serverConnector = testEyes.ServerConnector;
            Assert.AreEqual("someTestApiKey", serverConnector.ApiKey, nameof(serverConnector.ApiKey));
        }

        [Test]
        public void TestConfigServerUrl()
        {
            TestEyes testEyes = new TestEyes();
            IConfiguration config = testEyes.GetConfiguration();
            config.ServerUrl = "https://some.testurl.com/";
            testEyes.SetConfiguration(config);
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            IServerConnector serverConnector = testEyes.ServerConnector;
            Assert.AreEqual("https://some.testurl.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));
        }

        [Test]
        public void TestApiKeyProperty()
        {
            TestEyes testEyes = new TestEyes();
            testEyes.ApiKey = "someTestApiKey";
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            IServerConnector serverConnector = testEyes.ServerConnector;
            Assert.AreEqual("someTestApiKey", serverConnector.ApiKey, nameof(serverConnector.ApiKey));
        }

        [Test]
        public void TestServerUrlProperty()
        {
            TestEyes testEyes = new TestEyes();
            testEyes.ServerUrl = "https://some.testurl.com/";
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            IServerConnector serverConnector = testEyes.ServerConnector;
            Assert.AreEqual("https://some.testurl.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));
        }

        [TestCase(typeof(IConfiguration), typeof(Configuration))]
        [TestCase(typeof(Selenium.IConfiguration), typeof(Selenium.Configuration))]
        public void TestConfigurationCopyConstructor(Type iConfigType, Type configType)
        {
            object config = Activator.CreateInstance(configType);
            PropertyInfo[] properties = iConfigType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                object origValue = pi.GetValue(config);
                object modifiedValue = ModifyValue(pi.PropertyType, origValue);
                Assert.AreNotEqual(origValue, modifiedValue, "Member: {0} (type {1})", pi.Name, pi.PropertyType.Name);
                pi.SetValue(config, modifiedValue);
            }
            object copiedConfig = Activator.CreateInstance(configType, config);
            foreach (PropertyInfo pi in properties)
            {
                object origValue = pi.GetValue(config);
                object copiedValue = pi.GetValue(copiedConfig);
                Assert.AreEqual(origValue, copiedValue, "Member: {0} (type {1})", pi.Name, pi.PropertyType.Name);
            }
        }

        [Test]
        public void TestCheckSettingsClone()
        {
            ICheckSettings checkSettings = Target.Window();
            checkSettings = checkSettings.Accessibility(new Rectangle(20, 30, 40, 50), AccessibilityRegionType.GraphicalObject);
            checkSettings = checkSettings.BeforeRenderScreenshotHook("Test Hook");
            checkSettings = checkSettings.Content(new Rectangle(10, 20, 30, 40));
            checkSettings = checkSettings.EnablePatterns(true);
            checkSettings = checkSettings.Floating(new Rectangle(30, 40, 50, 60), 1, 2, 3, 4);
            checkSettings = checkSettings.Fully(true);
            checkSettings = checkSettings.Ignore(new Rectangle(40, 50, 60, 70));
            checkSettings = checkSettings.IgnoreCaret(true);
            checkSettings = checkSettings.IgnoreDisplacements(true);
            checkSettings = checkSettings.Layout(new Rectangle(50, 60, 70, 80));
            checkSettings = checkSettings.MatchLevel(MatchLevel.Exact);
            checkSettings = checkSettings.SendDom(true);
            checkSettings = checkSettings.Strict(new Rectangle(60, 70, 80, 90));
            checkSettings = checkSettings.Timeout(TimeSpan.FromMinutes(4));
            checkSettings = checkSettings.UseDom(true);
            checkSettings = checkSettings.WithName("Test Name");

            ICheckSettingsInternal clonedCheckSettings = (ICheckSettingsInternal)checkSettings.Clone();

            Assert.AreEqual(null, clonedCheckSettings.GetTargetRegion(), "target region");

            IGetAccessibilityRegion[] getAccessibilityRegions = clonedCheckSettings.GetAccessibilityRegions();
            Assert.AreEqual(1, getAccessibilityRegions.Length, "number of accessibility region getters");
            IList<AccessibilityRegionByRectangle> accessibilityRegions = getAccessibilityRegions[0].GetRegions(null, null);
            Assert.AreEqual(1, accessibilityRegions.Count, "number of accessibility regions");
            AccessibilityRegionByRectangle expectedAccessibilityRegion = new AccessibilityRegionByRectangle(new Rectangle(20, 30, 40, 50), AccessibilityRegionType.GraphicalObject);
            Assert.AreEqual(expectedAccessibilityRegion.Region, accessibilityRegions[0].Region, "accessibility region coordinates");
            Assert.AreEqual(expectedAccessibilityRegion.Type, accessibilityRegions[0].Type, "accessibility region type");

            Assert.AreEqual("Test Hook", clonedCheckSettings.GetScriptHooks()["beforeCaptureScreenshot"]);

            AssertRegion(clonedCheckSettings.GetContentRegions(), new Rectangle(10, 20, 30, 40), "Content");

            Assert.AreEqual(true, clonedCheckSettings.GetEnablePatterns());

            IGetFloatingRegion[] getFloatingRegions = clonedCheckSettings.GetFloatingRegions();
            Assert.AreEqual(1, getFloatingRegions.Length, "number of Floating region getters");
            IList<FloatingMatchSettings> FloatingRegions = getFloatingRegions[0].GetRegions(null, null);
            Assert.AreEqual(1, FloatingRegions.Count, "number of Floating regions");
            FloatingMatchSettings expectedFloatingRegion = new FloatingMatchSettings(30, 40, 50, 60, 1, 2, 3, 4);
            Assert.AreEqual(expectedFloatingRegion, FloatingRegions[0], "Floating region");

            Assert.AreEqual(true, clonedCheckSettings.GetStitchContent(), "fully (stitch content)");

            AssertRegion(clonedCheckSettings.GetIgnoreRegions(), new Rectangle(40, 50, 60, 70), "Ignore");

            Assert.AreEqual(true, clonedCheckSettings.GetIgnoreCaret(), "ignore caret");
            Assert.AreEqual(true, clonedCheckSettings.GetIgnoreDisplacements(), "ignore displacements");

            AssertRegion(clonedCheckSettings.GetLayoutRegions(), new Rectangle(50, 60, 70, 80), "Layout");

            Assert.AreEqual(MatchLevel.Exact, clonedCheckSettings.GetMatchLevel(), "match level");
            Assert.AreEqual(true, clonedCheckSettings.GetSendDom(), "send dom");

            AssertRegion(clonedCheckSettings.GetStrictRegions(), new Rectangle(60, 70, 80, 90), "Strict");

            Assert.AreEqual((int)TimeSpan.FromMinutes(4).TotalMilliseconds, clonedCheckSettings.GetTimeout(), "timeout");
            Assert.AreEqual(true, clonedCheckSettings.GetUseDom(), "use dom");

            Assert.AreEqual("Test Name", clonedCheckSettings.GetName(), "name");
        }

        [Test]
        public void TestBrowserNames()
        {
            List<BrowserType> browsers = new List<BrowserType>((BrowserType[])Enum.GetValues(typeof(BrowserType)));
            Assert.AreEqual(17, browsers.Count, "wrong number of browser types");
            foreach (BrowserType browser in browsers)
            {
                Assert.NotNull(BrowserNames.GetBrowserName(browser), $"{nameof(BrowserType)}.{browser} is not covered by {nameof(BrowserNames)}.{nameof(BrowserNames.GetBrowserName)}");
            }
            Assert.AreEqual(BrowserNames.Chrome, BrowserNames.GetBrowserName(BrowserType.CHROME));
            browsers.Remove(BrowserType.CHROME);
            Assert.AreEqual(BrowserNames.Chrome, BrowserNames.GetBrowserName(BrowserType.CHROME_ONE_VERSION_BACK));
            browsers.Remove(BrowserType.CHROME_ONE_VERSION_BACK);
            Assert.AreEqual(BrowserNames.Chrome, BrowserNames.GetBrowserName(BrowserType.CHROME_TWO_VERSIONS_BACK));
            browsers.Remove(BrowserType.CHROME_TWO_VERSIONS_BACK);

            Assert.AreEqual(BrowserNames.Firefox, BrowserNames.GetBrowserName(BrowserType.FIREFOX));
            browsers.Remove(BrowserType.FIREFOX);
            Assert.AreEqual(BrowserNames.Firefox, BrowserNames.GetBrowserName(BrowserType.FIREFOX_ONE_VERSION_BACK));
            browsers.Remove(BrowserType.FIREFOX_ONE_VERSION_BACK);
            Assert.AreEqual(BrowserNames.Firefox, BrowserNames.GetBrowserName(BrowserType.FIREFOX_TWO_VERSIONS_BACK));
            browsers.Remove(BrowserType.FIREFOX_TWO_VERSIONS_BACK);

            Assert.AreEqual(BrowserNames.Safari, BrowserNames.GetBrowserName(BrowserType.SAFARI));
            browsers.Remove(BrowserType.SAFARI);
            Assert.AreEqual(BrowserNames.Safari, BrowserNames.GetBrowserName(BrowserType.SAFARI_ONE_VERSION_BACK));
            browsers.Remove(BrowserType.SAFARI_ONE_VERSION_BACK);
            Assert.AreEqual(BrowserNames.Safari, BrowserNames.GetBrowserName(BrowserType.SAFARI_TWO_VERSIONS_BACK));
            browsers.Remove(BrowserType.SAFARI_TWO_VERSIONS_BACK);
            Assert.AreEqual(BrowserNames.Safari, BrowserNames.GetBrowserName(BrowserType.SAFARI_EARLY_ACCESS));
            browsers.Remove(BrowserType.SAFARI_EARLY_ACCESS);

            Assert.AreEqual(BrowserNames.IE + " 10", BrowserNames.GetBrowserName(BrowserType.IE_10));
            browsers.Remove(BrowserType.IE_10);
            Assert.AreEqual(BrowserNames.IE + " 11", BrowserNames.GetBrowserName(BrowserType.IE_11));
            browsers.Remove(BrowserType.IE_11);

            Assert.AreEqual(BrowserNames.Edge, BrowserNames.GetBrowserName(BrowserType.EDGE));
            browsers.Remove(BrowserType.EDGE);
            Assert.AreEqual(BrowserNames.Edge, BrowserNames.GetBrowserName(BrowserType.EDGE_LEGACY));
            browsers.Remove(BrowserType.EDGE_LEGACY);

            Assert.AreEqual(BrowserNames.EdgeChromium, BrowserNames.GetBrowserName(BrowserType.EDGE_CHROMIUM));
            browsers.Remove(BrowserType.EDGE_CHROMIUM);
            Assert.AreEqual(BrowserNames.EdgeChromium, BrowserNames.GetBrowserName(BrowserType.EDGE_CHROMIUM_ONE_VERSION_BACK));
            browsers.Remove(BrowserType.EDGE_CHROMIUM_ONE_VERSION_BACK);
            Assert.AreEqual(BrowserNames.EdgeChromium, BrowserNames.GetBrowserName(BrowserType.EDGE_CHROMIUM_TWO_VERSIONS_BACK));
            browsers.Remove(BrowserType.EDGE_CHROMIUM_TWO_VERSIONS_BACK);
            Assert.AreEqual(0, browsers.Count, "Not all browser types names has been verified. Remaining browser types: " + browsers.Concat(", "));
        }

        [Test]
        public void TestProxySettings()
        {
            ProxySettings proxySettings = new ProxySettings("http://127.0.0.1", 8888, "username", "password");
            Uri proxyUri = proxySettings.ProxyUri;
            Assert.AreEqual(new Uri("http://username:password@127.0.0.1:8888"), proxyUri);

            WebProxy proxy = new WebProxy(proxyUri);
            Assert.AreEqual(new Uri("http://username:password@127.0.0.1:8888"), proxy.Address);

            proxySettings = new ProxySettings("http://127.0.0.1");
            proxyUri = proxySettings.ProxyUri;
            Assert.AreEqual(new Uri("http://127.0.0.1:80"), proxyUri);
        }

        private static void AssertRegion(IGetRegions[] getRegions, Rectangle expectedRegion, string regionType)
        {
            Assert.AreEqual(1, getRegions.Length, "number of {0} region getters", regionType);
            IList<IMutableRegion> regions = getRegions[0].GetRegions(null, null);
            Assert.AreEqual(1, regions.Count, "number of {0} regions", regionType);
            Assert.AreEqual(expectedRegion, regions[0].Rectangle, "{0} region", regionType);
        }

        private object ModifyValue(Type type, object origValue)
        {
            object modifiedValue = origValue;
            if (type == typeof(string))
            {
                modifiedValue = origValue + "_dummy";
            }
            else if (type == typeof(bool))
            {
                modifiedValue = !(bool)origValue;
            }
            else if (type == typeof(bool?))
            {
                modifiedValue = ((bool?)origValue) ?? true;
                modifiedValue = !(bool)modifiedValue;
            }
            else if (type == typeof(BatchInfo))
            {
                modifiedValue = new BatchInfo((origValue as BatchInfo)?.Name + "_dummy");
            }
            else if (type == typeof(ImageMatchSettings))
            {
                modifiedValue = new ImageMatchSettings();
            }
            else if (type == typeof(TimeSpan))
            {
                modifiedValue = ((TimeSpan)origValue).Add(TimeSpan.FromSeconds(1));
            }
            else if (type == typeof(int))
            {
                modifiedValue = ((int)origValue) + 1;
            }
            else if (type == typeof(int?))
            {
                int? ov = ((int?)origValue);
                modifiedValue = ov.HasValue ? ov.Value + 1 : 1;
            }
            else if (type == typeof(RectangleSize))
            {
                RectangleSize origRectSize = origValue as RectangleSize;
                if (origRectSize == null)
                {
                    modifiedValue = new RectangleSize();
                }
                else
                {
                    modifiedValue = new RectangleSize(origRectSize.Width + 1, origRectSize.Height + 1);
                }
            }
            else if (type == typeof(MatchLevel))
            {
                modifiedValue = MatchLevel.Exact;
                if (modifiedValue == origValue)
                {
                    modifiedValue = MatchLevel.Content;
                }
            }
            else if (type == typeof(StitchModes))
            {
                modifiedValue = StitchModes.CSS;
                if (modifiedValue == origValue)
                {
                    modifiedValue = StitchModes.Scroll;
                }
            }
            else if (type == typeof(AccessibilityLevel))
            {
                modifiedValue = AccessibilityLevel.AA;
                if (modifiedValue == origValue)
                {
                    modifiedValue = AccessibilityLevel.AAA;
                }
            }
            else if (type == typeof(AccessibilitySettings))
            {
                modifiedValue = new AccessibilitySettings(AccessibilityLevel.AA, AccessibilityGuidelinesVersion.WCAG_2_0);
            }
            else if (type == typeof(VisualGridOption[]))
            {
                var list = new List<VisualGridOption>() { new VisualGridOption("option1", "value1") };
                if (origValue != null && list.Count == ((VisualGridOption[])origValue).Length)
                {
                    list.Add(new VisualGridOption("option2", true));
                }
                modifiedValue = list.ToArray();
            }
            else if (type == typeof(IList<int>))
            {
                var list = new int[] { 1, 2, 3 };
                if (origValue != null && list.Length == ((IList<int>)origValue).Count)
                {
                    list = new int[] { 1, 2, 3, 4 };
                }
                modifiedValue = list;
            }
            else if (type == typeof(WebProxy))
            {
                if (origValue == null)
                {
                    modifiedValue = new WebProxy("http://127.0.0.1:8888");
                }
                else
                {
                    modifiedValue = null;
                }
            }
            return modifiedValue;
        }
    }
}
