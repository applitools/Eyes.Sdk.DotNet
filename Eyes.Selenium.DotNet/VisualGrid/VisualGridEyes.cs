using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using Applitools.VisualGrid.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.VisualGrid
{
    public class VisualGridEyes : IVisualGridEyes, ISeleniumEyes, IUserActionsEyes
    {
        private static readonly string PROCESS_PAGE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePoll.js");
        private static readonly string PROCESS_PAGE_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePollForIE.js");
        private static readonly string POLL_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResult.js");
        private static readonly string POLL_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResultForIE.js");

        private static readonly int MB = 1024 * 1024;

        private static readonly string GET_ELEMENT_XPATH_JS =
            "const atName='data-applitools-element-id'; var el=arguments[0];" +
            "if (el.hasAttribute(atName)) { var id = el.getAttribute(atName) } " +
            "else { window.APPLITOOLS_ELEMENT_ID = window.APPLITOOLS_ELEMENT_ID || 0; " +
            " window.APPLITOOLS_ELEMENT_ID++; var id = window.APPLITOOLS_ELEMENT_ID;" +
            " el.setAttribute(atName, id); }" +
            "return '//*[@'+atName+'=\"'+id+'\"]';";

        internal readonly VisualGridRunner runner_;
        private readonly Dictionary<string, IRunningTest> testList_ = new Dictionary<string, IRunningTest>();
        private readonly List<RunningTest> testsInCloseProcess_ = new List<RunningTest>();
        private ICollection<Task<TestResultContainer>> closeFutures_ = new HashSet<Task<TestResultContainer>>();
        private IJavaScriptExecutor jsExecutor_;
        private string url_;
        internal Ufg.IDebugResourceWriter debugResourceWriter_;
        private bool? isDisabled_;
        private ISeleniumConfigurationProvider configProvider_;
        private IConfiguration configAtOpen_;
        private IWebDriver webDriver_;
        private EyesWebDriver driver_;
        internal UserAgent userAgent_;
        private readonly Dictionary<string, string> properties_ = new Dictionary<string, string>();
        private RectangleSize viewportSize_;
        private bool isOpen_;
        private readonly string eyesId_ = Guid.NewGuid().ToString();

        internal VisualGridEyes(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
        {
            ArgumentGuard.NotNull(visualGridRunner, nameof(visualGridRunner));
            if (visualGridRunner.GetAllTestResultsAlreadyCalled)
            {
                throw new InvalidOperationException("Runner already returned its results");
            }
            configProvider_ = configurationProvider;
            Logger = visualGridRunner.Logger;
            runner_ = visualGridRunner;

            IDebugResourceWriter drw = runner_.DebugResourceWriter;
            Ufg.IDebugResourceWriter ufgDrw = EyesSeleniumUtils.ConvertDebugResourceWriter(drw);
            debugResourceWriter_ = ufgDrw;
        }

        private Configuration Config_ { get => configProvider_.GetConfiguration(); }

        public string ApiKey
        {
            get => Config_.ApiKey ?? runner_.ApiKey;
            set => Config_.ApiKey = value;
        }

        public string ServerUrl
        {
            get => Config_.ServerUrl ?? runner_.ServerUrl;
            set => Config_.ServerUrl = value;
        }

        public WebProxy Proxy { get; set; }

        public Logger Logger { get; }

        public bool IsDisabled
        {
            get => isDisabled_ ?? runner_.IsDisabled;
            set => isDisabled_ = value;
        }

        protected string BaseAgentId => GetBaseAgentId();

        internal static string GetBaseAgentId()
        {
            Assembly assembly = typeof(VisualGridEyes).Assembly;
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"Eyes.Selenium.VisualGrid.DotNet/{versionInfo.ProductVersion}";
        }

        public string AgentId
        {
            get => Config_.AgentId;
            set => Config_.SetAgentId(value);
        }

        public string FullAgentId => (AgentId == null) ? BaseAgentId : $"{AgentId} [{BaseAgentId}]";

        public BatchInfo Batch
        {
            get => Config_.Batch ?? new BatchInfo();
            set => Config_.SetBatch(value);
        }

        public List<IUserAction> UserInputs { get; } = new List<IUserAction>();

        public bool IsEyesClosed()
        {
            return testList_.Values.All(t => t.IsCompleted);
        }

        public bool IsOpen => !IsEyesClosed();

        public ICollection<TestResultContainer> TestResults { get; } = new List<TestResultContainer>();

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

            string uaString = driver_.GetUserAgent();
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

            configAtOpen_ = GetConfigClone_();

            List<VisualGridRunningTest> newTests = new List<VisualGridRunningTest>();
            IServerConnector serverConnector = runner_.ServerConnector;
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
                    eyesId_, browserInfo, Logger, configProvider_, serverConnector);
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
            if (webDriver is IJavaScriptExecutor jsExecutor)
            {
                jsExecutor_ = jsExecutor;
            }
            if (webDriver is RemoteWebDriver remoteDriver)
            {
                driver_ = new EyesWebDriver(Logger, null, this, remoteDriver);
            }
            url_ = webDriver.Url;
        }

        public TestResults AbortIfNotClosed()
        {
            Logger.Verbose("enter");
            return Abort();
        }

        public TestResults Abort()
        {
            Logger.Verbose("enter");
            AbortAsync();
            return WaitForEyesToFinish_(false);
        }

        public void AbortAsync()
        {
            Logger.Verbose("enter");
            foreach (RunningTest runningTest in testList_.Values)
            {
                runningTest.IssueAbort(new EyesException("Eyes.Close wasn't called. Aborted the test"), false);
            }
            Logger.Verbose("exit");
        }

        private void Abort_(Exception e)
        {
            foreach (RunningTest runningTest in testList_.Values)
            {
                runningTest.IssueAbort(e, true);
            }
        }

        private IList<VisualGridSelector[]> GetRegionsXPaths_(ICheckSettings checkSettings)
        {
            List<VisualGridSelector[]> result = new List<VisualGridSelector[]>();
            ICheckSettingsInternal csInternal = (ICheckSettingsInternal)checkSettings;
            IList<Tuple<IWebElement, object>>[] elementLists = CollectSeleniumRegions_(csInternal);

            int i;
            for (i = 0; i < elementLists.Length; ++i)
            {
                IList<Tuple<IWebElement, object>> elementsList = elementLists[i];
                GetRegionsXPaths_(result, elementsList);
            }

            return result;
        }

        private void GetRegionsXPaths_(List<VisualGridSelector[]> result, IList<Tuple<IWebElement, object>> elementsList)
        {
            List<VisualGridSelector> xpaths = new List<VisualGridSelector>();
            foreach (Tuple<IWebElement, object> element in elementsList)
            {
                if (element.Item1 == null) continue;
                try
                {
                    string xpath = (string)jsExecutor_.ExecuteScript(GET_ELEMENT_XPATH_JS, element.Item1);
                    xpaths.Add(new VisualGridSelector(xpath, element.Item2));
                }
                catch (Exception e)
                {
                    CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, e, eyesId_);
                }
            }
            result.Add(xpaths.ToArray());
        }

        private IList<Tuple<IWebElement, object>>[] CollectSeleniumRegions_(ICheckSettingsInternal csInternal)
        {
            IGetRegions[] ignoreRegions = csInternal.GetIgnoreRegions();
            IGetRegions[] layoutRegions = csInternal.GetLayoutRegions();
            IGetRegions[] strictRegions = csInternal.GetStrictRegions();
            IGetRegions[] contentRegions = csInternal.GetContentRegions();
            IGetFloatingRegion[] floatingRegions = csInternal.GetFloatingRegions();
            IGetAccessibilityRegion[] accessibilityRegions = csInternal.GetAccessibilityRegions();

            IList<Tuple<IWebElement, object>> ignoreElements = GetElementsFromRegions_(ignoreRegions);
            IList<Tuple<IWebElement, object>> layoutElements = GetElementsFromRegions_(layoutRegions);
            IList<Tuple<IWebElement, object>> strictElements = GetElementsFromRegions_(strictRegions);
            IList<Tuple<IWebElement, object>> contentElements = GetElementsFromRegions_(contentRegions);
            IList<Tuple<IWebElement, object>> floatingElements = GetElementsFromRegions_(floatingRegions);
            IList<Tuple<IWebElement, object>> accessibilityElements = GetElementsFromRegions_(accessibilityRegions);

            IList<Tuple<IWebElement, object>> userActionElements = GetElementsFromUserActions_(UserInputs);

            return new IList<Tuple<IWebElement, object>>[] {
                ignoreElements,
                layoutElements,
                strictElements,
                contentElements,
                floatingElements,
                accessibilityElements,
                userActionElements };
        }

        private IList<Tuple<IWebElement, object>> GetElementsFromUserActions_(IList<IUserAction> userInputs)
        {
            List<Tuple<IWebElement, object>> elements = new List<Tuple<IWebElement, object>>();
            foreach (VGUserAction userInput in userInputs)
            {
                IWebElement element = (IWebElement)userInput.Control;
                elements.Add(new Tuple<IWebElement, object>(element, new SimpleRegionByElement(element)));
            }
            return elements;
        }

        private IList<Tuple<IWebElement, object>> GetElementsFromRegions_(IList regionsProvider)
        {
            List<Tuple<IWebElement, object>> elements = new List<Tuple<IWebElement, object>>();
            foreach (object getRegions in regionsProvider)
            {
                if (getRegions is IGetSeleniumRegion getSeleniumRegion)
                {
                    IList<IWebElement> webElements = getSeleniumRegion.GetElements(webDriver_);
                    foreach (IWebElement element in webElements)
                    {
                        elements.Add(new Tuple<IWebElement, object>(element, getRegions));
                    }
                }
            }
            return elements;
        }

        public void Check(params ICheckSettings[] checkSettings)
        {
            ArgumentGuard.IsValidState(IsOpen, "Eyes not open");
            if (!ValidateEyes_()) return;
            foreach (ICheckSettings checkSetting in checkSettings)
            {
                CheckImpl_(checkSetting);
            }
            UserInputs.Clear();
        }

        public void Check(ICheckSettings checkSettings)
        {
            CheckImpl_(checkSettings);
            UserInputs.Clear();
        }

        private void CheckImpl_(ICheckSettings checkSettings)
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
                IList<VisualGridSelector[]> regionsXPaths = GetRegionsXPaths_(checkSettings);
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

        internal Dictionary<int, List<RunningTest>> MapRunningTestsToRequiredBrowserWidth_(ISeleniumCheckTarget seleniumCheckSettings)
        {
            IList<int> layoutBreakpoints;
            bool isLayoutBreakpointsEnabled;
            if (seleniumCheckSettings.GetLayoutBreakpoints().Count > 0 ||
                seleniumCheckSettings.GetLayoutBreakpointsEnabled())
            {
                layoutBreakpoints = seleniumCheckSettings.GetLayoutBreakpoints();
                isLayoutBreakpointsEnabled = seleniumCheckSettings.GetLayoutBreakpointsEnabled();
            }
            else
            {
                layoutBreakpoints = GetConfigClone_().LayoutBreakpoints;
                isLayoutBreakpointsEnabled = GetConfigClone_().LayoutBreakpointsEnabled;
            }

            HashSet<string> testIds = new HashSet<string>();
            foreach (string runningTestId in testList_.Keys)
            {
                testIds.Add(runningTestId);
            }

            Dictionary<int, List<RunningTest>> requiredWidths = new Dictionary<int, List<RunningTest>>();
            if (isLayoutBreakpointsEnabled || layoutBreakpoints.Count > 0)
            {
                foreach (RunningTest runningTest in testList_.Values)
                {
                    int width = runningTest.BrowserInfo.GetDeviceSize().Width;
                    if (width <= 0)
                    {
                        width = viewportSize_.Width;
                    }

                    if (layoutBreakpoints.Count > 0)
                    {
                        for (int i = layoutBreakpoints.Count - 1; i >= 0; i--)
                        {
                            if (width >= layoutBreakpoints[i])
                            {
                                width = layoutBreakpoints[i];
                                break;
                            }
                        }

                        if (width < layoutBreakpoints[0])
                        {
                            width = layoutBreakpoints[0] - 1;
                            Logger.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                                new { message = $"Device width is smaller than the smallest breakpoint {layoutBreakpoints[0]}" });
                        }
                    }

                    if (requiredWidths.TryGetValue(width, out List<RunningTest> tests))
                    {
                        tests.Add(runningTest);
                    }
                    else
                    {
                        List<RunningTest> list = new List<RunningTest> { runningTest };
                        requiredWidths.Add(width, list);
                    }
                }
            }

            return requiredWidths;
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

        private Configuration GetConfigClone_()
        {
            return new Configuration(configProvider_.GetConfiguration());
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

        private ICheckSettings UpdateCheckSettings_(ICheckSettings checkSettings)
        {
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;

            MatchLevel? matchLevel = checkSettingsInternal.GetMatchLevel();
            bool? fully = checkSettingsInternal.GetStitchContent();
            bool? sendDom = checkSettingsInternal.GetSendDom();
            bool? useCookies = seleniumCheckTarget.GetUseCookies();
            var visualGridOptions = checkSettingsInternal.GetVisualGridOptions();

            if (matchLevel == null)
            {
                checkSettings = checkSettings.MatchLevel(Config_.MatchLevel);
            }

            if (fully == null)
            {
                checkSettings = checkSettings.Fully(Config_.IsForceFullPageScreenshot ?? checkSettingsInternal.IsCheckWindow());
            }

            if (sendDom == null)
            {
                checkSettings = checkSettings.SendDom(Config_.SendDom);
            }

            if (useCookies == null)
            {
                checkSettings = checkSettings.UseDom(Config_.UseCookies);
            }

            List<VisualGridOption> options = new List<VisualGridOption>();
            if (Config_.VisualGridOptions != null) options.AddRange(Config_.VisualGridOptions);
            if (visualGridOptions != null) options.AddRange(visualGridOptions);

            checkSettings = checkSettings.VisualGridOptions(options.Count > 0 ? options.ToArray() : null);

            return checkSettings;
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
            IConfiguration config, ISeleniumCheckTarget seleniumCheckTarget,
            VisualGridRunner runner, EyesWebDriver driver, Logger logger, params string[] testIds)
        {
            string domScript = userAgent.IsInternetExplorer ? PROCESS_PAGE_FOR_IE : PROCESS_PAGE;
            string pollingScript = userAgent.IsInternetExplorer ? POLL_RESULT_FOR_IE : POLL_RESULT;

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

            string result = EyesSeleniumUtils.RunDomScript(logger, driver, testIds, domScript, arguments, pollingArguments, pollingScript);
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
            ISeleniumCheckTarget seleniumCheckTarget,
            VisualGridRunner runner, EyesWebDriverTargetLocator switchTo, EyesWebDriver driver, Logger logger)
        {
            string[] testIds = runner.allEyes_.SelectMany(e => e.GetAllTests().Select(t => t.Key)).ToArray();

            if (seleniumCheckTarget?.GetUseCookies() ?? config.UseCookies)
            {
                ICookieJar cookieJar = driver.RemoteWebDriver.Manage().Cookies;
                var allCookies = cookieJar?.AllCookies;
                if (allCookies != null && allCookies.Count > 0)
                {
                    var cookieCollection = new CookieCollection();
                    foreach (var cookie in allCookies)
                    {
                        System.Net.Cookie snCookie = new System.Net.Cookie()
                        {
                            Name = cookie.Name,
                            Value = cookie.Value,
                            Path = cookie.Path,
                            Domain = cookie.Domain,
                            Secure = cookie.Secure,
                            HttpOnly = cookie.IsHttpOnly
                        };
                        cookieCollection.Add(snCookie);
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
                    switchTo.Frame(frame);
                    FrameData result = CaptureDomSnapshot_(switchTo, userAgent, config, seleniumCheckTarget, runner, driver, logger, testIds);
                    frameData.Frames.Add(result);
                    frameData.Cdt[crossFrame.Index].Attributes.Add(new AttributeData("data-applitools-src", result.Url.AbsoluteUri));
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

        public void SetLogHandler(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
        }

        public TestResults Close(bool throwEx = true)
        {
            CloseAsync();
            return WaitForEyesToFinish_(throwEx);
        }

        private TestResults WaitForEyesToFinish_(bool throwException)
        {
            while (!IsCompleted())
            {
                Thread.Sleep(500);
            }

            IList<TestResultContainer> allResults = GetAllTestResults();
            TestResultContainer errorResult = null;
            TestResults firstResult = null;
            foreach (TestResultContainer result in allResults)
            {
                if (firstResult == null)
                {
                    firstResult = result.TestResults;
                }
                if (result.Exception != null)
                {
                    errorResult = result;
                    break;
                }
            }

            if (errorResult != null)
            {
                if (throwException)
                {
                    throw new EyesException("Error occured during run:", errorResult.Exception);
                }
                return errorResult.TestResults;
            }

            return firstResult;
        }

        public bool IsCompleted()
        {
            return GetAllTestResults() != null;
        }

        internal bool IsCloseIssued => !isOpen_;

        public void CloseAsync()
        {
            if (!ValidateEyes_())
            {
                return;
            }

            isOpen_ = false;
            Logger.Log(TraceLevel.Notice, eyesId_, Stage.Close, StageType.Called,
                new { message = $"closing {testList_.Count} running tests" });
            foreach (RunningTest runningTest in testList_.Values)
            {
                VisualGridRunningTest vgRunningTest = (VisualGridRunningTest)runningTest;
                Logger.Verbose("running test name: {0}", Config_.TestName);
                Logger.Verbose("running test device info: {0}", vgRunningTest.BrowserInfo);
                Logger.Verbose("is current running test open: {0}", vgRunningTest.IsOpen);
                Logger.Verbose("is current running test ready to close: {0}", vgRunningTest.IsTestReadyToClose);
                Logger.Verbose("is current running test closed: {0}", ((IRunningTest)vgRunningTest).IsCompleted);
                Logger.Verbose("closing current running test");
                vgRunningTest.IssueClose();
            }
        }

        public void AddProperty(string name, string value)
        {
            properties_.Add(name, value);
        }

        public void ClearProperties()
        {
            properties_.Clear();
        }

        public string PrintAllFutures()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Task<TestResultContainer> future in closeFutures_)
            {
                sb.Append(future.Id).Append(" - ").Append(future.Status).Append(future.AsyncState).AppendLine();
            }
            if (sb.Length >= Environment.NewLine.Length) sb.Length -= Environment.NewLine.Length; // remove last new line
            return sb.ToString();
        }

        private bool ValidateEyes_()
        {
            if (IsDisabled)
            {
                Logger.Verbose("WARNING! Invalid Operation - Eyes Disabled!");
                return false;
            }
            return true;
        }

        public override string ToString()
        {
            return $"{nameof(VisualGridEyes)} (#{GetHashCode()}) - close future count: {closeFutures_.Count}";
        }

        public EyesWebDriver GetDriver()
        {
            return this.driver_;
        }

        public IBatchCloser GetBatchCloser()
        {
            return testList_.Values.FirstOrDefault();
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

        public IList<TestResultContainer> GetAllTestResults()
        {
            List<TestResultContainer> allResults = new List<TestResultContainer>();
            foreach (RunningTest runningTest in testList_.Values)
            {
                if (!((IRunningTest)runningTest).IsCompleted)
                {
                    if (runner_.GetError() != null)
                    {
                        throw new EyesException("Execution crashed", runner_.GetError());
                    }
                    return null;
                }

                allResults.Add(runningTest.GetTestResultContainer());
            }

            return allResults;
        }

        public IDictionary<string, IRunningTest> GetAllTests()
        {
            return new Dictionary<string, IRunningTest>(testList_);
        }
    }
}
