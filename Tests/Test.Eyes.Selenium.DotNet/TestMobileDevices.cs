using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using System;
using System.Collections;
using System.Collections.Generic;
using ScreenOrientation = OpenQA.Selenium.ScreenOrientation;

namespace Applitools.Selenium.Tests
{
    //[TestFixture]
    //[Parallelizable]
    //[TestFixtureSource(nameof(TestsMobilePages))]
    public class TestMobileDevices : ReportingTestSuite
    {
        private readonly string page_;

        public TestMobileDevices(string page)
        {
            suiteArgs_.Add("page", page);
            page_ = page;
        }

        public static IEnumerable TestsMobilePages
        {
            get
            {
                yield return new object[] { "mobile" };
                yield return new object[] { "desktop" };
                yield return new object[] { "scrolled_mobile" };
            }
        }


        public static IEnumerable AndroidDeviceFixtureArgs
        {
            get
            {
                yield return new TestCaseData("Android Emulator", "8.0", ScreenOrientation.Portrait);
                //yield return new TestCaseData("Android Emulator", "8.0", ScreenOrientation.Landscape);
                //yield return new TestCaseData("Android Emulator", "8.0", ScreenOrientation.Portrait);
                //yield return new TestCaseData("Android Emulator", "8.0", ScreenOrientation.Landscape);
            }
        }

        private static Dictionary<string, ChromeMobileEmulationDeviceSettings> chromeSimulationData_;
        private static object lockObject_ = new object();

        public static IEnumerable IOSDeviceFixtureArgs
        {
            get
            {
                #region iPads, Landscape

                // resolution: 2048 x 1536 ; viewport: 2048 x 1408 
                // [iPad Air 2, 10.3]
                yield return new TestCaseData("iPad Air 2 Simulator", "10.3", ScreenOrientation.Landscape);

                // resolution: 2048 x 1536 ; viewport: 2048 x 1396
                // [iPad Air 2, 12.0] [iPad Air 2, 11.0] [iPad Air, 12.0] [iPad Air, 11.0] [iPad Pro (9.7 inch), 11.0] [iPad, 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "12.0", ScreenOrientation.Landscape);

                // resolution: 2048 x 1536 ; viewport: 2048 x 1331
                // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "11.3", ScreenOrientation.Landscape);

                // resolution: 2732 x 2048 ; viewport: 2732 x 1908
                // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
                yield return new TestCaseData("iPad Pro (12.9 inch) (2nd generation) Simulator", "11.0", ScreenOrientation.Landscape);

                // resolution: 2224 x 1668 ; viewport: 2224 x 1528
                // [iPad Pro (10.5 inch), 11.0]
                yield return new TestCaseData("iPad Pro (10.5 inch) Simulator", "11.0", ScreenOrientation.Landscape);
                #endregion

                #region iPads, Portrait

                // resolution: 1536 x 2048; viewport: 1536 x 1843
                // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
                yield return new TestCaseData("iPad (5th generation) Simulator", "11.0", ScreenOrientation.Portrait);

                // resolution: 1536 x 2048; viewport: 1536 x 1920 
                // [iPad Air 2, 10.3]
                yield return new TestCaseData("iPad Air 2 Simulator", "10.3", ScreenOrientation.Portrait);

                // resolution: 1536 x 2048; viewport: 1536 x 1908 
                // [iPad Air 2, 11.0] [iPad Air 2, 12.0] [iPad Air, 11.0] [iPad, 11.0] [iPad Pro (9.7 inch), 12.0] [iPad Pro (9.7 inch), 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "11.0", ScreenOrientation.Portrait);

                // resolution: 2048 x 2732 ; viewport: 2048 x 2592
                // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
                yield return new TestCaseData("iPad Pro (12.9 inch) (2nd generation) Simulator", "11.0", ScreenOrientation.Portrait);

                // resolution: 1668 x 2224 ; viewport: 1668 x 2084
                // [iPad Pro (10.5 inch), 11.0]
                yield return new TestCaseData("iPad Pro (10.5 inch) Simulator", "11.0", ScreenOrientation.Portrait);

                #endregion

                #region iPhones, Landscape

                // resolution: 2436 x 1125 ; viewport: 2172 x 912
                // [iPhone XS, 12.2] [iPhone X, 11.2]
                yield return new TestCaseData("iPhone XS Simulator", "12.2", ScreenOrientation.Landscape);

                // resolution: 2436 x 1125 ; viewport: 2172 x 813
                // [iPhone 11 Pro, 13.0]
                yield return new TestCaseData("iPhone 11 Pro Simulator", "13.0", ScreenOrientation.Landscape);

                // resolution: 2688 x 1242 ; viewport: 2424 x 1030
                // [iPhone XS Max, 12.2]
                yield return new TestCaseData("iPhone XS Max Simulator", "12.2", ScreenOrientation.Landscape);

                // resolution: 2688 x 1242 ; viewport: 2424 x 922
                // [iPhone 11 Pro Max, 13.0]
                yield return new TestCaseData("iPhone 11 Pro Max Simulator", "13.0", ScreenOrientation.Landscape);

                // resolution: 1792 x 828 ; viewport: 1616 x 686
                // [iPhone XR, 12.2]
                yield return new TestCaseData("iPhone XR Simulator", "12.2", ScreenOrientation.Landscape);

                // resolution: 1792 x 828 ; viewport: 1616 x 620
                // [iPhone 11, 13.0]
                yield return new TestCaseData("iPhone 11 Simulator", "13.0", ScreenOrientation.Landscape);

                // resolution: 2208 x 1242 ; viewport: 2208 x 1092
                // [iPhone 6 Plus, 11.0]
                yield return new TestCaseData("iPhone 6 Plus Simulator", "11.0", ScreenOrientation.Landscape);

                // resolution: 1334 x 750 ; viewport: 1334 x 662
                // [iPhone 7, 10.3]
                yield return new TestCaseData("iPhone 7 Simulator", "10.3", ScreenOrientation.Landscape);

                // resolution: 2208 x 1242 ; viewport: 2208 x 1110
                // [iPhone 7 Plus, 10.3]
                yield return new TestCaseData("iPhone 7 Plus Simulator", "10.3", ScreenOrientation.Landscape);

                // resolution: 1136 x 640 ; viewport: 1136 x 464
                // [iPhone 5s, 10.3]
                yield return new TestCaseData("iPhone 5s Simulator", "10.3", ScreenOrientation.Landscape);

                #endregion

                #region iPhones, Portrait

                // resolution: 1125 x 2436 ; viewport: 1125 x 1905
                // [iPhone XS, 12.2] [iPhone X, 11.2] [iPhone 11 Pro, 13.0]
                yield return new TestCaseData("iPhone XS Simulator", "12.2", ScreenOrientation.Portrait);

                // resolution: 1242 x 2688 ; viewport: 1242 x 2157
                // [iPhone XS Max, 12.2] [iPhone 11 Pro Max, 13.0]
                yield return new TestCaseData("iPhone XS Max Simulator", "12.2", ScreenOrientation.Portrait);

                // resolution: 828 x 1792 ; viewport: 828 x 1438
                // [iPhone XR, 12.2] [iPhone 11, 13.0]
                yield return new TestCaseData("iPhone XR Simulator", "12.2", ScreenOrientation.Portrait);

                // resolution: 1242 x 2208 ; viewport: 1242 x 1866
                // [iPhone 6 Plus, 11.0] [iPhone 7 Plus, 10.3]
                yield return new TestCaseData("iPhone 6 Plus Simulator", "11.0", ScreenOrientation.Portrait);

                // resolution: 750 x 1334 ; viewport: 750 x 1118
                // [iPhone 7, 10.3]
                yield return new TestCaseData("iPhone 7 Simulator", "10.3", ScreenOrientation.Portrait);

                // resolution: 640 x 1136 ; viewport: 640 x 920
                // [iPhone 5s, 10.3]
                yield return new TestCaseData("iPhone 5s Simulator", "10.3", ScreenOrientation.Portrait);

                #endregion
            }
        }

        //[Test]
        //[TestCaseSource(nameof(IOSDeviceFixtureArgs))]
        public void TestIOSSafariStitch(string deviceName, string platformVersion, ScreenOrientation deviceOrientation)
        {
            InitEyes_(deviceName, platformVersion, deviceOrientation, "iOS", "Safari", page_);
        }

        //[Test]
        //[TestCaseSource(nameof(AndroidDeviceFixtureArgs))]
        public void TestAndroidStitch(string deviceName, string platformVersion, ScreenOrientation deviceOrientation)
        {
            InitEyes_(deviceName, platformVersion, deviceOrientation, "Android", "Chrome", page_);
        }

        private static void InitEyes_(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, string platformName, string browserName, string page)
        {
            Eyes eyes = new Eyes();

            eyes.Batch = TestDataProvider.BatchInfo;
            eyes.SaveNewTests = false;
            eyes.StitchMode = StitchModes.CSS;

            eyes.AddProperty("Orientation", deviceOrientation.ToString());
            eyes.AddProperty("Page", page);

            string testName = GetTestName_(deviceName, platformVersion, deviceOrientation, page);

            TestUtils.SetupLogging(eyes, testName);

            eyes.Logger.Log(TraceLevel.Info, testName, Stage.TestFramework, StageType.Start);
            IWebDriver driver = InitEyesSimulation_(deviceName, platformVersion, deviceOrientation, $"{testName} ({eyes.FullAgentId})", platformName, browserName);

            if (driver != null)
            {
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
                RunTest_(true, eyes, testName, driver, page);
            }
            else
            {
                eyes.Logger.Log(TraceLevel.Error, testName, Stage.TestFramework,
                    new { message = "failed to create webdriver." });
            }
        }

        private static IWebDriver InitOnSaucelabs(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, string testName, string platformName, string browserName)
        {
            AppiumOptions options = new AppiumOptions();

            options.AddAdditionalCapability(MobileCapabilityType.PlatformName, platformName);
            options.AddAdditionalCapability(MobileCapabilityType.PlatformVersion, platformVersion);
            options.AddAdditionalCapability(MobileCapabilityType.DeviceName, deviceName);
            options.AddAdditionalCapability(MobileCapabilityType.BrowserName, browserName);
            options.AddAdditionalCapability("deviceOrientation", deviceOrientation.ToString().ToLower());

            options.AddAdditionalCapability("username", TestDataProvider.SAUCE_USERNAME);
            options.AddAdditionalCapability("accesskey", TestDataProvider.SAUCE_ACCESS_KEY);

            options.AddAdditionalCapability("name", testName);
            options.AddAdditionalCapability("idleTimeout", 360);

            options.PlatformName = platformName;

            IWebDriver webDriver = SeleniumUtils.RetryCreateWebDriver(() =>
            {
                IWebDriver driver = new RemoteWebDriver(new Uri(TestDataProvider.SAUCE_SELENIUM_URL), options.ToCapabilities(), TimeSpan.FromMinutes(4));
                return driver;
            });
            return webDriver;
        }

        private static IWebDriver InitEyesSimulation_(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, string testName, string platformName, string browserName)
        {
            if (chromeSimulationData_ == null)
            {
                lock (lockObject_)
                {
                    if (chromeSimulationData_ == null)
                    {
                        InitChromeSimulationData_();
                    }
                }
            }
            ChromeMobileEmulationDeviceSettings mobileSettings = chromeSimulationData_[$"{deviceName};{platformVersion};{deviceOrientation}"];
            IWebDriver driver = null;
            if (mobileSettings != null)
            {
                ChromeOptions options = new ChromeOptions();
                options.EnableMobileEmulation(mobileSettings);
                driver = SeleniumUtils.CreateChromeDriver(options);
            }
            return driver;
        }

        private static void InitChromeSimulationData_()
        {
            var chromeSimulationData = new Dictionary<string, ChromeMobileEmulationDeviceSettings>();

            #region Android
            chromeSimulationData["Android Emulator;8.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Android SDK built for x86_64 Build/OSR1.180418.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Mobile Safari/537.36",
                Width = 384,
                Height = 512,
                PixelRatio = 2
            };
            #endregion

            #region iPads, Landscape

            // resolution: 2048 x 1536 ; viewport: 2048 x 1408 
            // [iPad Air 2, 10.3]
            chromeSimulationData["iPad Air 2 Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 352,
                PixelRatio = 4
            };

            // resolution: 2048 x 1536 ; viewport: 2048 x 1396
            // [iPad Air 2, 12.0] [iPad Air 2, 11.0] [iPad Air, 12.0] [iPad Air, 11.0] [iPad Pro (9.7 inch), 11.0] [iPad, 11.0]
            chromeSimulationData["iPad Air 2 Simulator;12.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 349,
                PixelRatio = 4
            };

            // resolution: 2048 x 1536 ; viewport: 2048 x 1331
            // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
            chromeSimulationData["iPad Air 2 Simulator;11.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 333,
                PixelRatio = 4
            };

            // resolution: 2732 x 2048 ; viewport: 2732 x 1908
            // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
            chromeSimulationData["iPad Pro (12.9 inch) (2nd generation) Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 683,
                Height = 477,
                PixelRatio = 4
            };

            // resolution: 2224 x 1668 ; viewport: 2224 x 1528
            // [iPad Pro (10.5 inch), 11.0]
            chromeSimulationData["iPad Pro (10.5 inch) Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 556,
                Height = 382,
                PixelRatio = 4
            };
            #endregion

            #region iPads, Portrait

            // resolution: 1536 x 2048; viewport: 1536 x 1843
            // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
            chromeSimulationData["iPad (5th generation) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 922,
                PixelRatio = 2
            };

            // resolution: 1536 x 2048; viewport: 1536 x 1920 
            // [iPad Air 2, 10.3]
            chromeSimulationData["iPad Air 2 Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 960,
                PixelRatio = 2
            };

            // resolution: 1536 x 2048; viewport: 1536 x 1908 
            // [iPad Air 2, 11.0] [iPad Air 2, 12.0] [iPad Air, 11.0] [iPad, 11.0] [iPad Pro (9.7 inch), 12.0] [iPad Pro (9.7 inch), 11.0]
            chromeSimulationData["iPad Air 2 Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 954,
                PixelRatio = 2
            };

            // resolution: 2048 x 2732 ; viewport: 2048 x 2592
            // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
            chromeSimulationData["iPad Pro (12.9 inch) (2nd generation) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 1024,
                Height = 1296,
                PixelRatio = 2
            };

            // resolution: 1668 x 2224 ; viewport: 1668 x 2084
            // [iPad Pro (10.5 inch), 11.0]
            chromeSimulationData["iPad Pro (10.5 inch) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 834,
                Height = 1042,
                PixelRatio = 2
            };

            #endregion

            #region iPhones, Landscape

            // resolution: 2436 x 1125 ; viewport: 2172 x 912
            // [iPhone XS, 12.2] [iPhone X, 11.2]
            chromeSimulationData["iPhone XS Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 724,
                Height = 304,
                PixelRatio = 3
            };

            // resolution: 2436 x 1125 ; viewport: 2172 x 813
            // [iPhone 11 Pro, 13.0]
            chromeSimulationData["iPhone 11 Pro Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 724,
                Height = 271,
                PixelRatio = 3
            };


            // resolution: 2688 x 1242 ; viewport: 2424 x 1030
            // [iPhone XS Max, 12.2]
            chromeSimulationData["iPhone XS Max Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 344,
                PixelRatio = 3
            };

            // resolution: 2688 x 1242 ; viewport: 2424 x 922
            // [iPhone 11 Pro Max, 13.0]
            chromeSimulationData["iPhone 11 Pro Max Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 307,
                PixelRatio = 3
            };

            // resolution: 1792 x 828 ; viewport: 1616 x 686
            // [iPhone XR, 12.2]
            chromeSimulationData["iPhone XR Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 343,
                PixelRatio = 2
            };

            // resolution: 1792 x 828 ; viewport: 1616 x 620
            // [iPhone 11, 13.0]
            chromeSimulationData["iPhone 11 Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 310,
                PixelRatio = 2
            };

            // resolution: 2208 x 1242 ; viewport: 2208 x 1092
            // [iPhone 6 Plus, 11.0]
            chromeSimulationData["iPhone 6 Plus Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 736,
                Height = 364,
                PixelRatio = 3
            };

            // resolution: 1334 x 750 ; viewport: 1334 x 662
            // [iPhone 7, 10.3]
            chromeSimulationData["iPhone 7 Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 667,
                Height = 331,
                PixelRatio = 2
            };

            // resolution: 2208 x 1242 ; viewport: 2208 x 1110
            // [iPhone 7 Plus, 10.3]
            chromeSimulationData["iPhone 7 Plus Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 736,
                Height = 370,
                PixelRatio = 3
            };

            // resolution: 1136 x 640 ; viewport: 1136 x 464
            // [iPhone 5s, 10.3]
            chromeSimulationData["iPhone 5s Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 568,
                Height = 232,
                PixelRatio = 2
            };

            #endregion

            #region iPhones, Portrait

            // resolution: 1125 x 2436 ; viewport: 1125 x 1905
            // [iPhone XS, 12.2] [iPhone X, 11.2] [iPhone 11 Pro, 13.0]
            chromeSimulationData["iPhone XS Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 375,
                Height = 635,
                PixelRatio = 3
            };

            // resolution: 1242 x 2688 ; viewport: 1242 x 2157
            // [iPhone XS Max, 12.2] [iPhone 11 Pro Max, 13.0]
            chromeSimulationData["iPhone XS Max Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 719,
                PixelRatio = 3
            };

            // resolution: 828 x 1792 ; viewport: 828 x 1438
            // [iPhone XR, 12.2] [iPhone 11, 13.0]
            chromeSimulationData["iPhone XR Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 719,
                PixelRatio = 2
            }; ;

            // resolution: 1242 x 2208 ; viewport: 1242 x 1866
            // [iPhone 6 Plus, 11.0]
            chromeSimulationData["iPhone 6 Plus Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 622,
                PixelRatio = 3
            };

            // resolution: 750 x 1334 ; viewport: 750 x 1118
            // [iPhone 7, 10.3]
            chromeSimulationData["iPhone 7 Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 375,
                Height = 559,
                PixelRatio = 2
            };

            // resolution: 640 x 1136 ; viewport: 640 x 920
            // [iPhone 5s, 10.3]
            chromeSimulationData["iPhone 5s Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 320,
                Height = 460,
                PixelRatio = 2
            };

            #endregion

            chromeSimulationData_ = chromeSimulationData;
        }

        private static TestResults RunTest_(bool fully, Eyes eyes, string testName, IWebDriver driver, string page)
        {
            try
            {
                driver.Url = $"https://applitools.github.io/demo/TestPages/DynamicResolution/{page}.html";
                eyes.Open(driver, "Eyes Selenium SDK - iOS Safari Cropping", testName);
                //eyes.Check("Initial view", Target.Region(By.CssSelector("div.page")).Fully(fully).SendDom(false));
                eyes.Check(Target.Window().Fully(fully));
                TestResults result = eyes.Close();
                return result;
            }
            finally
            {
                driver.Quit();
                eyes.Abort();
            }
        }

        private static void ReportResultToSaucelabs(Eyes eyes, TestResults result)
        {
            HttpRestClient client = new HttpRestClient(new Uri("https://saucelabs.com/"));
            client.SetBasicAuth(TestDataProvider.SAUCE_USERNAME, TestDataProvider.SAUCE_ACCESS_KEY);
            PassedResult passed = new PassedResult(result.IsPassed);
            client.PutJson($"rest/v1/{TestDataProvider.SAUCE_USERNAME}/jobs/{((SeleniumEyes)eyes.activeEyes_).GetDriver().RemoteWebDriver.SessionId}", passed);
        }

        private static string GetTestName_(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, string page)
        {
            return $"{deviceName} {platformVersion} {deviceOrientation} {page} fully";
        }

        private class PassedResult
        {
            public PassedResult(bool isPassed)
            {
                Passed = isPassed;
            }

            public bool Passed { get; set; }
        }
    }
}
