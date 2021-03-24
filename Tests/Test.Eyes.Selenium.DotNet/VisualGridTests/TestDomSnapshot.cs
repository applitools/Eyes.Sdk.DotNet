using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using Cookie = System.Net.Cookie;

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

                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Cookies);


                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Cookies);

                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("frame2", "1", "/demo/TestPages/CookiesTestPage/subdir", "applitools.github.io"){
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

                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Cookies);


                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    } },
                    scriptResult.Frames[0].Cookies);

                CollectionAssert.AreEquivalent(new Cookie[] {
                    new Cookie("frame1", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("index", "1", "/demo/TestPages/CookiesTestPage", "applitools.github.io"){
                        HttpOnly = false,
                        Secure = false
                    },
                    new Cookie("frame2", "1", "/demo/TestPages/CookiesTestPage/subdir", "applitools.github.io"){
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

        [Test]
        public void TestIsCookieForUrlNonExpiredWithDottedCorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Expires = DateTime.MaxValue,
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/")));
        }

        [Test]
        public void TestNotIsCookieForUrlExpiredWithDottedCorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Expires = new DateTime(1),
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithDottedCorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithDottedCorrectDomainIgnorePort()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com:8080/")));
        }

        [Test]
        public void TestIsCookieForUrlWithDottedCorrectSubdomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://b.a.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithNotDottedCorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/")));
        }


        [Test]
        public void TestNotIsCookieForUrlWithNotDottedCorrectSubdomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://b.a.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithDottedIncorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://b.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithNotDottedIncorrectDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://b.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithNotDottedIncorrectSuffixedDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://ba.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithDottedIncorrectSuffixedDomain()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", ".a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://ba.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithSecureCookieNonSecureUrl()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = true
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/subdir")));
        }

        [Test]
        public void TestIsCookieForUrlWithSecureCookieSecureUrl()
        {
            Cookie cookie = new Cookie("subdir", "1", "/", "a.com")
            {
                Secure = true
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("https://a.com/subdir")));
        }

        [Test]
        public void TestIsCookieForUrlWithPathCookieIncorrectUrl()
        {
            Cookie cookie = new Cookie("subdir", "1", "/b", "a.com")
            {
                Secure = false
            };
            Assert.IsFalse(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/")));
        }

        [Test]
        public void TestIsCookieForUrlWithPathCookieCorrectSubdirUrl()
        {
            Cookie cookie = new Cookie("subdir", "1", "/b", "a.com")
            {
                Secure = false
            };
            Assert.IsTrue(ResourceFetcher.IsCookieForUrl_(cookie, new Uri("http://a.com/b/c")));
        }
    }
}
