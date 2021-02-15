using OpenQA.Selenium;
using System;
using OpenQA.Selenium.Chrome;
using Applitools.Selenium;
using System.Collections.Generic;
using Applitools.Tests.Utils;
using System.Runtime.CompilerServices;

namespace Applitools.Generated.Selenium.Tests
{
    public abstract class TestSetupGeneratedMobileEmulation : TestSetupGenerated
    {
        private static Dictionary<string, ChromeMobileEmulationDeviceSettings> chromeSimulationData_;
        private static object lockObject_ = new object();

        protected void SetUpDriver(string deviceName, string platformVersion, string platformName, ScreenOrientation deviceOrientation = ScreenOrientation.Portrait)
        {
            driver = InitEyesSimulation_(deviceName, platformVersion, deviceOrientation, platformName);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30);
        }

        protected void initEyes(string page, ScreenOrientation deviceOrientation = ScreenOrientation.Portrait)
        {
            string testName = NUnit.Framework.TestContext.CurrentContext.Test.MethodName;
            ILogHandler logHandler = TestUtils.InitLogHandler(testName);
            eyes = new Eyes(logHandler);
            initEyesSettings(false, true);
            eyes.AddProperty("Orientation", deviceOrientation.ToString());
            eyes.AddProperty("Page", page);
        }

        private static IWebDriver InitEyesSimulation_(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, string platformName)
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
                driver = CreateChromeDriver(options, headless: true);
            }
            return driver;
        }

        private static void InitChromeSimulationData_()
        {
            chromeSimulationData_ = new Dictionary<string, ChromeMobileEmulationDeviceSettings>();

            #region Android
            chromeSimulationData_["Android Emulator;8.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
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
            chromeSimulationData_["iPad Air 2 Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 352,
                PixelRatio = 4
            };

            // resolution: 2048 x 1536 ; viewport: 2048 x 1396
            // [iPad Air 2, 12.0] [iPad Air 2, 11.0] [iPad Air, 12.0] [iPad Air, 11.0] [iPad Pro (9.7 inch), 11.0] [iPad, 11.0]
            chromeSimulationData_["iPad Air 2 Simulator;12.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 349,
                PixelRatio = 4
            };

            // resolution: 2048 x 1536 ; viewport: 2048 x 1331
            // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
            chromeSimulationData_["iPad Air 2 Simulator;11.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 512,
                Height = 333,
                PixelRatio = 4
            };

            // resolution: 2732 x 2048 ; viewport: 2732 x 1908
            // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
            chromeSimulationData_["iPad Pro (12.9 inch) (2nd generation) Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 11_0_1 like Mac OS X) AppleWebKit/604.2.10 (KHTML, like Gecko) Version/11.0 Mobile/15A8401 Safari/604.1",
                Width = 683,
                Height = 477,
                PixelRatio = 4
            };

            // resolution: 2224 x 1668 ; viewport: 2224 x 1528
            // [iPad Pro (10.5 inch), 11.0]
            chromeSimulationData_["iPad Pro (10.5 inch) Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
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
            chromeSimulationData_["iPad (5th generation) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 922,
                PixelRatio = 2
            };

            // resolution: 1536 x 2048; viewport: 1536 x 1920 
            // [iPad Air 2, 10.3]
            chromeSimulationData_["iPad Air 2 Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 960,
                PixelRatio = 2
            };

            // resolution: 1536 x 2048; viewport: 1536 x 1908 
            // [iPad Air 2, 11.0] [iPad Air 2, 12.0] [iPad Air, 11.0] [iPad, 11.0] [iPad Pro (9.7 inch), 12.0] [iPad Pro (9.7 inch), 11.0]
            chromeSimulationData_["iPad Air 2 Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 768,
                Height = 954,
                PixelRatio = 2
            };

            // resolution: 2048 x 2732 ; viewport: 2048 x 2592
            // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
            chromeSimulationData_["iPad Pro (12.9 inch) (2nd generation) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPad; CPU OS 10_3 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E269 Safari/602.1",
                Width = 1024,
                Height = 1296,
                PixelRatio = 2
            };

            // resolution: 1668 x 2224 ; viewport: 1668 x 2084
            // [iPad Pro (10.5 inch), 11.0]
            chromeSimulationData_["iPad Pro (10.5 inch) Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
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
            chromeSimulationData_["iPhone XS Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 724,
                Height = 304,
                PixelRatio = 3
            };

            // resolution: 2436 x 1125 ; viewport: 2172 x 813
            // [iPhone 11 Pro, 13.0]
            chromeSimulationData_["iPhone 11 Pro Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 724,
                Height = 271,
                PixelRatio = 3
            };


            // resolution: 2688 x 1242 ; viewport: 2424 x 1030
            // [iPhone XS Max, 12.2]
            chromeSimulationData_["iPhone XS Max Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 344,
                PixelRatio = 3
            };

            // resolution: 2688 x 1242 ; viewport: 2424 x 922
            // [iPhone 11 Pro Max, 13.0]
            chromeSimulationData_["iPhone 11 Pro Max Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 307,
                PixelRatio = 3
            };

            // resolution: 1792 x 828 ; viewport: 1616 x 686
            // [iPhone XR, 12.2]
            chromeSimulationData_["iPhone XR Simulator;12.2;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 343,
                PixelRatio = 2
            };

            // resolution: 1792 x 828 ; viewport: 1616 x 620
            // [iPhone 11, 13.0]
            chromeSimulationData_["iPhone 11 Simulator;13.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 808,
                Height = 310,
                PixelRatio = 2
            };

            // resolution: 2208 x 1242 ; viewport: 2208 x 1092
            // [iPhone 6 Plus, 11.0]
            chromeSimulationData_["iPhone 6 Plus Simulator;11.0;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 736,
                Height = 364,
                PixelRatio = 3
            };

            // resolution: 1334 x 750 ; viewport: 1334 x 662
            // [iPhone 7, 10.3]
            chromeSimulationData_["iPhone 7 Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 667,
                Height = 331,
                PixelRatio = 2
            };

            // resolution: 2208 x 1242 ; viewport: 2208 x 1110
            // [iPhone 7 Plus, 10.3]
            chromeSimulationData_["iPhone 7 Plus Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 736,
                Height = 370,
                PixelRatio = 3
            };

            // resolution: 1136 x 640 ; viewport: 1136 x 464
            // [iPhone 5s, 10.3]
            chromeSimulationData_["iPhone 5s Simulator;10.3;Landscape"] = new ChromeMobileEmulationDeviceSettings()
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
            chromeSimulationData_["iPhone XS Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 375,
                Height = 635,
                PixelRatio = 3
            };

            // resolution: 1242 x 2688 ; viewport: 1242 x 2157
            // [iPhone XS Max, 12.2] [iPhone 11 Pro Max, 13.0]
            chromeSimulationData_["iPhone XS Max Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 719,
                PixelRatio = 3
            };

            // resolution: 828 x 1792 ; viewport: 828 x 1438
            // [iPhone XR, 12.2] [iPhone 11, 13.0]
            chromeSimulationData_["iPhone XR Simulator;12.2;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 719,
                PixelRatio = 2
            }; ;

            // resolution: 1242 x 2208 ; viewport: 1242 x 1866
            // [iPhone 6 Plus, 11.0]
            chromeSimulationData_["iPhone 6 Plus Simulator;11.0;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 414,
                Height = 622,
                PixelRatio = 3
            };

            // resolution: 750 x 1334 ; viewport: 750 x 1118
            // [iPhone 7, 10.3]
            chromeSimulationData_["iPhone 7 Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 375,
                Height = 559,
                PixelRatio = 2
            };

            // resolution: 640 x 1136 ; viewport: 640 x 920
            // [iPhone 5s, 10.3]
            chromeSimulationData_["iPhone 5s Simulator;10.3;Portrait"] = new ChromeMobileEmulationDeviceSettings()
            {
                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0 Mobile/15E148 Safari/604.1",
                Width = 320,
                Height = 460,
                PixelRatio = 2
            };

            #endregion
        }

    }
}
