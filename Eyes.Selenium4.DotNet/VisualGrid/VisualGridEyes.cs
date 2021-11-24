using Applitools.Fluent;
using Applitools.Selenium.Common;
using Applitools.Selenium.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Applitools.Selenium.VisualGrid
{
    public class VisualGridEyes : VisualGridEyesBase<IWebElement, IWebDriver>, ISeleniumEyes, IUserActionsEyes
    {
        private EyesWebDriver driver_;

        internal VisualGridEyes(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
            : base(configurationProvider, visualGridRunner)
        { }

        public IWebDriver Open(IWebDriver driver, string appName, string testName, Size viewportSize)
        {
            Config_.SetAppName(appName);
            Config_.SetTestName(testName);
            Config_.SetViewportSize(viewportSize);
            return Open(driver);
        }

        public IWebDriver Open(IWebDriver webDriver)
        {
            if (!ValidateEyes_()) return webDriver;

            Logger.GetILogHandler()?.Open();

            Logger.Log(TraceLevel.Notice, eyesId_, Stage.Open, StageType.Called,
                new { FullAgentId, DotNetVersion = CommonUtils.GetDotNetVersion() });

            ArgumentGuard.NotNull(webDriver, nameof(webDriver));

            string apiKey = ApiKey;
            EyesBase.ValidateAPIKey(apiKey);

            ArgumentGuard.NotEmpty(Config_.AppName, "appName");
            ArgumentGuard.NotEmpty(Config_.TestName, "testName");
            if (isOpen_)
            {
                Logger.Log(TraceLevel.Warn, eyesId_, Stage.Open, StageType.Called,
                    new { message = "called open more than once! Ignoring" });
                return webDriver_ != null ? webDriver_ : webDriver;
            }

            isOpen_ = true;
            InitDriver(webDriver);

            string uaString = webDriver.GetUserAgent();
            if (uaString != null)
            {
                Logger.Log(TraceLevel.Notice, eyesId_, Stage.Open, StageType.Called, new { userAgent = uaString });
                userAgent_ = UserAgent.ParseUserAgentString(uaString, true);
            }

            EnsureViewportSize_();

            Logger.Log(TraceLevel.Info, eyesId_, Stage.Open, StageType.Called,
                new { appName = Config_.AppName, testName = Config_.TestName, viewportSize = viewportSize_ });

            closeFutures_.Clear();

            if (Config_.Batch == null)
            {
                Config_.SetBatch(Batch);
            }

            List<RenderBrowserInfo> browserInfoList = Config_.GetBrowsersInfo();
            if (browserInfoList.Count == 0)
            {
                DesktopBrowserInfo desktopBrowserInfo = new DesktopBrowserInfo(viewportSize_);
                browserInfoList.Add(new RenderBrowserInfo(desktopBrowserInfo));
            }

            if (runner_.AgentId == null)
            {
                runner_.AgentId = FullAgentId;
            }

            configAtOpen_ = GetConfigClone();

            List<VisualGridRunningTest> newTests = new List<VisualGridRunningTest>();
            IServerConnector serverConnector = runner_.ServerConnector;
            string agentRunId = configAtOpen_.TestName + "_" + Guid.NewGuid();
            foreach (RenderBrowserInfo browserInfo in browserInfoList)
            {
                if (browserInfo.EmulationInfo as ChromeEmulationInfo != null)
                {
                    ChromeEmulationInfo emulationInfo = (ChromeEmulationInfo)browserInfo.EmulationInfo;
                    Dictionary<DeviceName, DeviceSize> deviceSizes = serverConnector.GetEmulatedDevicesSizes();
                    deviceSizes.TryGetValue(emulationInfo.DeviceName, out DeviceSize size);
                    browserInfo.SetEmulationDeviceSize(size);
                }
                if (browserInfo.IosDeviceInfo != null)
                {
                    Dictionary<IosDeviceName, DeviceSize> deviceSizes = serverConnector.GetIosDevicesSizes();
                    deviceSizes.TryGetValue(browserInfo.IosDeviceInfo.DeviceName, out DeviceSize size);
                    browserInfo.SetIosDeviceSize(size);
                }
                VisualGridRunningTest test = new VisualGridRunningTest(
                    eyesId_, browserInfo, Logger, configProvider_, serverConnector, agentRunId);
                testList_.Add(test.TestId, test);
                newTests.Add(test);
            }

            Logger.Log(TraceLevel.Info, eyesId_, Stage.Open, StageType.Called, new { testCount = testList_.Count });
            runner_.Open(this, newTests);
            return driver_ ?? webDriver;
        }

        internal void InitDriver(IWebDriver webDriver)
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
                //Bitmap bufferedImage = imageProvider.getImage();
                //debugScreenshotsProvider.save(bufferedImage, $"snapshot_{viewportSize}");
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

        private ICheckSettings SwitchFramesAsNeeded_(ICheckSettings checkSettings, EyesWebDriverTargetLocator switchTo,
            int switchedToCount)
        {
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;
            bool isFullPage = IsFullPage_(checkSettingsInternal);
            if (switchedToCount > 0 && isFullPage &&
                seleniumCheckTarget.GetTargetSelector() == null && seleniumCheckTarget.GetTargetElement() == null)
            {
                FrameChain frameChain = driver_.GetFrameChain().Clone();
                Frame frame = frameChain.Pop();
                checkSettings = ((SeleniumCheckSettings)checkSettings).Region(frame.Reference);
                switchTo.ParentFrame();
            }
            return checkSettings;
        }

        private bool IsFullPage_(ICheckSettingsInternal checkSettingsInternal)
        {
            bool isFullPage = true;
            bool? b;
            if ((b = checkSettingsInternal.GetStitchContent()) != null)
            {
                isFullPage = b.Value;
            }
            else if ((b = Config_.IsForceFullPageScreenshot) != null)
            {
                isFullPage = b.Value;
            }
            return isFullPage;
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

        private void EnsureViewportSize_()
        {
            RectangleSize viewportSize = Config_.ViewportSize;

            if (viewportSize == null || viewportSize.IsEmpty())
            {
                List<RenderBrowserInfo> browserInfoList = Config_.GetBrowsersInfo();
                if (browserInfoList != null && browserInfoList.Count > 0)
                {
                    foreach (RenderBrowserInfo deviceInfo in browserInfoList)
                    {
                        if (deviceInfo.EmulationInfo != null)
                        {
                            continue;
                        }
                        viewportSize = new RectangleSize(deviceInfo.Width, deviceInfo.Height);
                        break;
                    }
                }
            }

            if (viewportSize == null || viewportSize.IsEmpty())
            {
                viewportSize = EyesSeleniumUtils.GetViewportSize(Logger, driver_);
            }
            else
            {
                EyesSeleniumUtils.SetViewportSize(Logger, driver_, viewportSize);
            }

            viewportSize_ = viewportSize;
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

        internal static FrameData CaptureDomSnapshot_(EyesWebDriverTargetLocator switchTo, UserAgent userAgent,
            IConfiguration config, ISeleniumCheckTargetCommon seleniumCheckTarget,
            VisualGridRunner runner, EyesWebDriver driver, Logger logger, params string[] testIds)
        {
            string domScript = userAgent.IsInternetExplorer ? Common.Resources.PROCESS_PAGE_FOR_IE : Common.Resources.PROCESS_PAGE;
            string pollingScript = userAgent.IsInternetExplorer ? Common.Resources.POLL_PP_RESULT_FOR_IE : Common.Resources.POLL_PP_RESULT;

            bool keepOriginalUrls = runner.ServerConnector.GetType().Name.Contains("Mock");

            int chunkByteLength = userAgent.IsiOS ? 10 * MB : 256 * MB;
            object arguments = new
            {
                serializeResources = true,
                //skipResources = runner.CachedBlobsURLs.Keys,
                dontFetchResources = config.DisableBrowserFetching || (seleniumCheckTarget?.GetUseCookies() ?? config.UseCookies),
                chunkByteLength,
                //uniqueUrl = "(url, query) => url"
            };

            object pollingArguments = new { chunkByteLength };

            string result = Capture.Utils.RunDomScript(logger, driver, testIds, domScript, arguments, pollingArguments, pollingScript);
            if (keepOriginalUrls)
            {
                Regex removeQueryParameter = new Regex("\\?applitools-iframe=\\d*", RegexOptions.Compiled);
                result = removeQueryParameter.Replace(result, string.Empty);
            }
            FrameData frameData = JsonConvert.DeserializeObject<FrameData>(result);
            AnalyzeFrameData_(frameData, userAgent, config, seleniumCheckTarget, runner, switchTo, driver, logger);
            return frameData;
        }

        private static void AnalyzeFrameData_(FrameData frameData, UserAgent userAgent, IConfiguration config,
            ISeleniumCheckTargetCommon seleniumCheckTarget,
            VisualGridRunner runner, EyesWebDriverTargetLocator switchTo, EyesWebDriver driver, Logger logger)
        {
            string[] testIds = runner.allEyes_.SelectMany(e => e.GetAllTests().Select(t => t.Key)).ToArray();

            if (seleniumCheckTarget?.GetUseCookies() ?? config.UseCookies)
            {
                ICookieJar cookieJar = driver.WebDriver.Manage().Cookies;
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

                    frameData.Cookies = cookieCollection;
                }
            }

            FrameChain frameChain = driver.GetFrameChain().Clone();
            foreach (FrameData.CrossFrame crossFrame in frameData.CrossFrames)
            {
                if (crossFrame.Selector == null)
                {
                    logger.Verbose("cross frame with null selector");
                    continue;
                }

                try
                {
                    IWebElement frame = driver.FindElement(By.CssSelector(crossFrame.Selector));
                    var activeElementBeforeSwitch = switchTo.ActiveElement();
                    switchTo.Frame(frame);
                    var activeElementAfterSwitch = switchTo.ActiveElement();
                    if (!activeElementAfterSwitch.Equals(activeElementBeforeSwitch))
                    {
                        FrameData result = CaptureDomSnapshot_(switchTo, userAgent, config, seleniumCheckTarget, runner, driver, logger, testIds);
                        frameData.Frames.Add(result);
                        frameData.Cdt[crossFrame.Index].Attributes.Add(new AttributeData("data-applitools-src", result.Url.AbsoluteUri));
                    }
                    else
                    {
                        throw new Exception("Failed switching into frame");
                    }
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(logger, Stage.ResourceCollection, StageType.Failed, ex,
                        new { crossFrame.Selector }, testIds);
                }
                finally
                {
                    switchTo.Frames(frameChain);
                }
            }

            foreach (FrameData frame in frameData.Frames)
            {
                if (frame.Selector == null)
                {
                    logger.Verbose("cross frame with null selector");
                    continue;
                }

                try
                {
                    IWebElement frameElement = driver.FindElement(By.CssSelector(frame.Selector));
                    switchTo.Frame(frameElement);
                    AnalyzeFrameData_(frame, userAgent, config, seleniumCheckTarget, runner, switchTo, driver, logger);
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(logger, Stage.ResourceCollection, StageType.Failed, ex,
                        new { frame.Selector }, testIds);
                }
                finally
                {
                    switchTo.Frames(frameChain);
                }
            }
        }

        public EyesWebDriver GetDriver()
        {
            return this.driver_;
        }

        void IUserActionsEyes.AddKeyboardTrigger(IWebElement element, string text)
        {
            VGTextTrigger trigger = new VGTextTrigger(element, text);
            UserInputs.Add(trigger);
        }

        void IUserActionsEyes.AddMouseTrigger(MouseAction action, IWebElement element, Point cursor)
        {
            VGMouseTrigger trigger = new VGMouseTrigger(action, element, cursor);
            UserInputs.Add(trigger);
        }

        protected override object CreateSimpleRegionByElement(IWebElement element)
        {
            return new SimpleRegionByElement(element);
        }
    }
}
