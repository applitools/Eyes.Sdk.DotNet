using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace Applitools.Selenium.VisualGrid
{
    public class VisualGridEyes : VisualGridEyesBase<IWebElement, IWebDriver, ICookieJar, FrameChain>, ISeleniumEyes, IUserActionsEyes
    {
        private EyesWebDriver driver_;

        internal VisualGridEyes(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
            : base(configurationProvider, visualGridRunner)
        { }

        internal override void InitDriver(IWebDriver webDriver)
        {
            webDriver_ = webDriver;
            if (webDriver is IEyesJsExecutor eyesJsExecutor)
            {
                jsExecutor_ = eyesJsExecutor;
            }
            else if (webDriver is IJavaScriptExecutor)
            {
                jsExecutor_ = new SeleniumJavaScriptExecutor(webDriver);
            }
            if (webDriver is WebDriver wDriver)
            {
                driver_ = new EyesWebDriver(Logger, null, this, wDriver);
            }
            url_ = webDriver.Url;
        }

        protected override string GetUserAgent()
        {
            return webDriver_.GetUserAgent();
        }

        protected override string BaseAgentId => GetBaseAgentId();

        internal override void CheckImpl_(ICheckSettings checkSettings)
        {
            if (!ValidateEyes_()) return;

            Logger.Log(TraceLevel.Notice, eyesId_, Stage.Check, StageType.Called);

            try
            {
                object logMessage = runner_.GetConcurrencyLog();
                if (logMessage != null)
                {
                    NetworkLogHandler.SendEvent(runner_.ServerConnector, TraceLevel.Notice, logMessage);
                }
            }
            catch (JsonException e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, e, eyesId_);
            }

            FrameChain originalFC = driver_.GetFrameChain().Clone();
            EyesWebDriverTargetLocator switchTo = ((EyesWebDriverTargetLocator)driver_.SwitchTo());
            try
            {
                ArgumentGuard.NotOfType(checkSettings, typeof(ICheckSettings), nameof(checkSettings));
                if (checkSettings is IImplicitInitialization seleniumCheckTarget)
                {
                    seleniumCheckTarget.Init(Logger, driver_);
                }

                int waitBeforeScreenshots = Config_.WaitBeforeScreenshots;
                Thread.Sleep(waitBeforeScreenshots);
                int switchedToCount = SwitchToFrame_((ISeleniumCheckTarget)checkSettings);
                TrySetTargetSelector_((SeleniumCheckSettings)checkSettings);
                checkSettings = UpdateCheckSettings_(checkSettings);
                IList<VisualGridSelector[]> regionsXPaths = GetRegionsXPaths(checkSettings);
                ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
                SeleniumCheckSettings seleniumCheckSettings = (SeleniumCheckSettings)checkSettings;
                string source = webDriver_.Url;

                Dictionary<int, List<RunningTest>> requiredWidths = MapRunningTestsToRequiredBrowserWidth_(seleniumCheckSettings);
                if (requiredWidths.Count == 0)
                {
                    CaptureDomForResourceCollection_(0, testList_.Values, switchTo, checkSettingsInternal, regionsXPaths, source);
                    return;
                }
                Size originalSize = driver_.Manage().Window.Size;
                foreach (KeyValuePair<int, List<RunningTest>> entry in requiredWidths)
                {
                    CaptureDomForResourceCollection_(entry.Key, entry.Value, switchTo, checkSettingsInternal, regionsXPaths, source);
                }
                driver_.Manage().Window.Size = originalSize;
            }
            catch (Exception e)
            {
                foreach (RunningTest runningTest in testList_.Values)
                {
                    runningTest.SetTestInExceptionMode(e);
                }
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, e, testList_.Keys.ToArray());
            }
            finally
            {
                switchTo.Frames(originalFC);
            }
        }

        private void CaptureDomForResourceCollection_(int requiredWidth, IEnumerable<IRunningTest> tests,
              EyesWebDriverTargetLocator switchTo, ICheckSettingsInternal checkSettingsInternal,
              IList<VisualGridSelector[]> regionsXPaths, string source)
        {
            HashSet<string> testIds = new HashSet<string>();
            foreach (RunningTest runningTest in tests)
            {
                testIds.Add(runningTest.TestId);
            }
            if (requiredWidth != 0)
            {
                try
                {
                    EyesSeleniumUtils.SetViewportSize(Logger, webDriver_, new RectangleSize(requiredWidth, viewportSize_.Height));
                    Thread.Sleep(300);
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, StageType.DomScript, ex, testIds);
                }
            }

            if (requiredWidth != 0)
            {
                Size viewportSize = EyesSeleniumUtils.GetViewportSize(Logger, webDriver_);
                Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript, new { requiredWidth, viewportSize });
            }

            FrameData scriptResult = CaptureDomSnapshot_(switchTo, userAgent_, configAtOpen_,
                (ISeleniumCheckTarget)checkSettingsInternal, runner_, driver_, Logger, eyesId_);

            Uri[] blobsUrls = scriptResult.Blobs.Select(b => b.Url).ToArray();

            Logger.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript,
                new { regionsXPaths, blobsUrls, scriptResult.ResourceUrls, cdtCount = scriptResult.Cdt.Count });

            List<CheckTask> checkTasks = new List<CheckTask>();
            foreach (RunningTest runningTest in tests)
            {
                if (runningTest.IsCloseTaskIssued)
                {
                    continue;
                }

                checkTasks.Add((CheckTask)runningTest.IssueCheck(
                    (ICheckSettings)checkSettingsInternal, regionsXPaths, source, UserInputs));
            }

            scriptResult.UserAgent = userAgent_;
            //visualGridRunner_.DebugResourceWriter = Config_.DebugResourceWriter;
            runner_.Check(scriptResult, checkTasks);
            //Logger.Verbose("created renderTask  ({0})", checkSettings);
        }

        private int SwitchToFrame_(ISeleniumCheckTarget checkTarget)
        {
            if (checkTarget == null)
            {
                return 0;
            }

            IList<FrameLocator> frameChain = checkTarget.GetFrameChain();
            int switchedToFrameCount = 0;
            foreach (FrameLocator frameLocator in frameChain)
            {
                if (SwitchToFrame_(frameLocator))
                {
                    switchedToFrameCount++;
                }
            }
            return switchedToFrameCount;
        }

        private bool SwitchToFrame_(ISeleniumFrameCheckTarget frameTarget)
        {
            ITargetLocator switchTo = driver_.SwitchTo();
            int? frameIndex = frameTarget.GetFrameIndex();
            if (frameIndex != null)
            {
                switchTo.Frame(frameIndex.Value);
                return true;
            }

            string frameNameOrId = frameTarget.GetFrameNameOrId();
            if (frameNameOrId != null)
            {
                switchTo.Frame(frameNameOrId);
                return true;
            }

            IWebElement frameReference = frameTarget.GetFrameReference();
            if (frameReference != null)
            {
                switchTo.Frame(frameReference);
                return true;
            }

            By frameSelector = frameTarget.GetFrameSelector();
            if (frameSelector != null)
            {
                IWebElement frameElement = webDriver_.FindElement(frameSelector);
                if (frameElement != null)
                {
                    switchTo.Frame(frameElement);
                    return true;
                }
            }

            return false;
        }

        protected override FrameChain GetFrameChain(IWebDriver driver)
        {
            return ((EyesWebDriver)driver).GetFrameChain().Clone();
        }

        protected override IWebElement FindElement(IWebDriver driver, string selector)
        {
            return driver.FindElement(By.CssSelector(selector));
        }

        protected override IList<ICookie> CollectCookies(FrameData frameData, IWebDriver driver)
        {
            ICookieJar cookieJar = driver.Manage().Cookies;
            var allCookies = cookieJar?.AllCookies;
            if (allCookies != null && allCookies.Count > 0)
            {
                var cookieCollection = new List<ICookie>();
                foreach (var seleniumCookie in allCookies)
                {
                    Cookie cookie = new Cookie()
                    {
                        Name = seleniumCookie.Name,
                        Value = seleniumCookie.Value,
                        Path = seleniumCookie.Path,
                        Domain = seleniumCookie.Domain,
                        Secure = seleniumCookie.Secure,
                        HttpOnly = seleniumCookie.IsHttpOnly,
                        Expiry = seleniumCookie.Expiry
                    };
                    cookieCollection.Add(cookie);
                }
                return cookieCollection;
            }
            return null;
        }

        protected override RectangleSize EnsureViewportSize()
        {
            RectangleSize viewportSize = base.EnsureViewportSize();

            if (viewportSize == null || viewportSize.IsEmpty())
            {
                viewportSize = EyesSeleniumUtils.GetViewportSize(Logger, driver_);
            }
            else
            {
                EyesSeleniumUtils.SetViewportSize(Logger, driver_, viewportSize);
            }

            return viewportSize;
        }

        private void TrySetTargetSelector_(SeleniumCheckSettings checkSettings)
        {
            ISeleniumCheckTarget seleniumCheckTarget = checkSettings;
            IWebElement element = seleniumCheckTarget.GetTargetElement();
            if (element == null)
            {
                By targetSelector = seleniumCheckTarget.GetTargetSelector();
                if (targetSelector != null)
                {
                    element = webDriver_.FindElement(targetSelector);
                }
            }

            if (element == null) return;

            string xpath = (string)jsExecutor_.ExecuteScript(GET_ELEMENT_XPATH_JS, element);
            VisualGridSelector vgs = new VisualGridSelector(xpath, "target");
            checkSettings.SetTargetSelector(vgs);
        }

        public EyesWebDriver GetDriver()
        {
            return driver_;
        }

        protected override IWebDriver GetDriverInternal()
        {
            return driver_;
        }

        protected override object CreateSimpleRegionByElement(IWebElement element)
        {
            return new SimpleRegionByElement(element);
        }
    }
}
