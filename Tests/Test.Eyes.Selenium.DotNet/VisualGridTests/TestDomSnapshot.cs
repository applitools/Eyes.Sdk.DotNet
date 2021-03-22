using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public class TestDomSnapshot : ReportingTestSuite
    {

        [Test]
        public void TestCreateDomSnapshotCollectsCookiesWhenHandlingCorsFrames()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo/TestPages/CookiesTestPage/";
            Eyes eyes = Setup(driver);
            VisualGridRunner runner = (VisualGridRunner)eyes.runner_;
            try
            {
                Configuration config = eyes.GetConfiguration();
                config
                    .SetAppName("test app").SetTestName("test name")
                    .SetBatch(TestDataProvider.BatchInfo)
                    .SetUseCookies(true);
                eyes.SetConfiguration(config);
                EyesWebDriver eyesDriver = (EyesWebDriver)eyes.Open(driver);
                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)eyesDriver.SwitchTo();
                UserAgent userAgent = UserAgent.ParseUserAgentString(eyesDriver.GetUserAgent());
                FrameData scriptResult = VisualGridEyes.CaptureDomSnapshot_(
                    switchTo, userAgent, config, runner, eyesDriver, runner.Logger);

                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Cookies);


                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Cookies);

                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("frame2", "1", "/demo/TestPages/CookiesTestPage/subdir", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Frames[0].Cookies);
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
                runner.StopServiceRunner();
            }
        }
        /*

def test_create_dom_snapshot_collects_cookies_when_handling_cors_frames(driver):
    driver = EyesWebDriver(driver, None)
    driver.get("http://applitools.github.io/demo/TestPages/CookiesTestPage/")

    dom = create_dom_snapshot(driver, logger, False, [], 10000, True, True)

    assert dom["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
    ]
    assert dom["frames"][0]["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
    ]
    assert dom["frames"][0]["frames"][0]["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame2",
            "path": "/demo/TestPages/CookiesTestPage/subdir",
            "secure": False,
            "value": "1",
        },
    ]
        */

        [Test]
        public void TestCreateDomSnapshotCollectsCookiesWhenNotHandlingCorsFrames()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo/TestPages/CookiesTestPage/";
            Eyes eyes = Setup(driver);
            VisualGridRunner runner = (VisualGridRunner)eyes.runner_;
            try
            {
                Configuration config = eyes.GetConfiguration();
                config
                    .SetAppName("test app").SetTestName("test name")
                    .SetBatch(TestDataProvider.BatchInfo)
                    .SetUseCookies(true);
                eyes.SetConfiguration(config);
                EyesWebDriver eyesDriver = (EyesWebDriver)eyes.Open(driver);
                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)eyesDriver.SwitchTo();
                UserAgent userAgent = UserAgent.ParseUserAgentString(eyesDriver.GetUserAgent());
                FrameData scriptResult = VisualGridEyes.CaptureDomSnapshot_(
                    switchTo, userAgent, config, runner, eyesDriver, runner.Logger);

                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Cookies);


                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Cookies);

                CollectionAssert.AreEquivalent(new System.Net.Cookie[] {
                    new System.Net.Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new System.Net.Cookie("frame2", "1", "/demo/TestPages/CookiesTestPage/subdir", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Frames[0].Cookies);
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
                runner.StopServiceRunner();
            }
        }
        /*
def test_create_dom_snapshot_collects_cookies_when_not_handling_cors_frames(driver):
    driver = EyesWebDriver(driver, None)
    driver.get("http://applitools.github.io/demo/TestPages/CookiesTestPage/")

    dom = create_dom_snapshot(driver, logger, True, [], 10000, False, True)

    assert dom["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
    ]
    assert dom["frames"][0]["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
    ]
    assert dom["frames"][0]["frames"][0]["cookies"] == [
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame1",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "index",
            "path": "/demo/TestPages/CookiesTestPage",
            "secure": False,
            "value": "1",
        },
        {
            "domain": "applitools.github.io",
            "httpOnly": False,
            "name": "frame2",
            "path": "/demo/TestPages/CookiesTestPage/subdir",
            "secure": False,
            "value": "1",
        },
    ]
        */

        [Test]
        public void TestCreateDomSnapshotCollectsCookiesWhenDisabled()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "http://applitools.github.io/demo/TestPages/CookiesTestPage/";
            Eyes eyes = Setup(driver);
            VisualGridRunner runner = (VisualGridRunner)eyes.runner_;
            try
            {
                Configuration config = eyes.GetConfiguration();
                config.SetAppName("test app").SetTestName("test name").SetBatch(TestDataProvider.BatchInfo);
                eyes.SetConfiguration(config);
                EyesWebDriver eyesDriver = (EyesWebDriver)eyes.Open(driver);
                EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)eyesDriver.SwitchTo();
                UserAgent userAgent = UserAgent.ParseUserAgentString(eyesDriver.GetUserAgent());
                FrameData scriptResult = VisualGridEyes.CaptureDomSnapshot_(
                    switchTo, userAgent, config, runner, eyesDriver, runner.Logger);

                Assert.IsNull(scriptResult.Cookies);
                Assert.IsNull(scriptResult.Frames[0].Cookies);
                Assert.IsNull(scriptResult.Frames[0].Frames[0].Cookies);
            }
            finally
            {
                driver.Quit();
                eyes.AbortIfNotClosed();
                runner.StopServiceRunner();
            }
        }

        private static Eyes Setup(IWebDriver driver)
        {
            ILogHandler logHandler = TestUtils.InitLogHandler();
            WebDriverProvider webdriverProvider = new WebDriverProvider();
            IServerConnectorFactory serverConnectorFactory = new MockServerConnectorFactory(webdriverProvider);
            VisualGridRunner runner = new VisualGridRunner(10, "test", serverConnectorFactory, logHandler);
            Eyes eyes = new Eyes(runner);
            webdriverProvider.SetDriver(driver);
            return eyes;
        }

        /*
def test_create_dom_snapshot_doesnt_collect_cookies_when_disabled(driver):
    driver = EyesWebDriver(driver, None)
    driver.get("http://applitools.github.io/demo/TestPages/CookiesTestPage/")

    dom = create_dom_snapshot(driver, logger, True, [], 10000, True, False)

    assert "cookies" not in dom
    assert "cookies" not in dom["frames"][0]
    assert "cookies" not in dom["frames"][0]["frames"][0]
         */
    }
}
