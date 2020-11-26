using Applitools.Fluent;
using Applitools.Selenium.Fluent;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
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
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.VisualGrid
{
    public class VisualGridEyes : IEyes, ISeleniumEyes, IVisualGridEyes
    {
        private static readonly string PROCESS_PAGE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.Resources.processPagePoll.js");
        private static readonly string PROCESS_PAGE_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.Resources.processPagePollForIE.js");
        private static readonly string POLL_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.Resources.pollResult.js");
        private static readonly string POLL_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.Resources.pollResultForIE.js");

        private static readonly int MB = 1024 * 1024;

        private static readonly string GET_ELEMENT_XPATH_JS =
            "var el = arguments[0];" +
            "var xpath = '';" +
            "do {" +
            " var parent = el.parentElement;" +
            " var index = 1;" +
            " if (parent !== null) {" +
            "  var children = parent.children;" +
            "  for (var childIdx in children) {" +
            "    var child = children[childIdx];" +
            "    if (child === el) break;" +
            "    if (child.tagName === el.tagName) index++;" +
            "  }" +
            "}" +
            "xpath = '/' + el.tagName + '[' + index + ']' + xpath;" +
            " el = parent;" +
            "} while (el !== null);" +
            "return '/' + xpath;";

        private readonly VisualGridRunner visualGridRunner_;
        private readonly List<RunningTest> testList_ = new List<RunningTest>();
        private readonly List<RunningTest> testsInCloseProcess_ = new List<RunningTest>();
        private ICollection<Task<TestResultContainer>> closeFutures_ = new HashSet<Task<TestResultContainer>>();
        private RenderingInfo renderingInfo_;
        internal IUfgConnector eyesConnector_;
        private IJavaScriptExecutor jsExecutor_;
        private string url_;
        private EyesListener listener_;
        private readonly RunningTest.RunningTestListener testListener_;
#pragma warning disable CS0414
        private bool hasEyesIssuedOpenTasks_;  // for debugging
#pragma warning restore CS0414
        internal Ufg.IDebugResourceWriter debugResourceWriter_;
        private bool? isDisabled_;
        private ISeleniumConfigurationProvider configProvider_;
        private IConfiguration configAtOpen_;
        private IWebDriver webDriver_;
        private EyesWebDriver driver_;
        internal UserAgent userAgent_;
        private readonly Dictionary<string, string> properties_ = new Dictionary<string, string>();
        private RectangleSize viewportSize_;

        internal VisualGridEyes(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
        {
            ArgumentGuard.NotNull(visualGridRunner, nameof(visualGridRunner));
            configProvider_ = configurationProvider;
            Logger = visualGridRunner.Logger;
            visualGridRunner_ = visualGridRunner;

            IDebugResourceWriter drw = visualGridRunner_.DebugResourceWriter;
            if (drw is FileDebugResourceWriter fileDRW)
            {
                debugResourceWriter_ = new Ufg.FileDebugResourceWriter(fileDRW.TargetFolder);
            }
            debugResourceWriter_ = debugResourceWriter_ ?? NullDebugResourceWriter.Instance;

            testListener_ = new RunningTest.RunningTestListener(
                (task, test) =>
                {
                    if (task.TaskType == TaskType.Close || task.TaskType == TaskType.Abort)
                    {
                        hasEyesIssuedOpenTasks_ = false;
                    }
                    listener_?.OnTaskComplete(task, this);
                },

                () => listener_?.OnRenderComplete());
        }
        private Configuration Config_ { get => configProvider_.GetConfiguration(); }

        public string ApiKey
        {
            get => Config_.ApiKey ?? visualGridRunner_.ApiKey;
            set => Config_.ApiKey = value;
        }

        public string ServerUrl
        {
            get
            {
                if (eyesConnector_ != null)
                {
                    string uri = eyesConnector_.ServerUrl;
                    if (uri != null) return uri;
                }
                return Config_.ServerUrl ?? visualGridRunner_.ServerUrl;
            }
            set => Config_.ServerUrl = value;
        }

        public WebProxy Proxy { get; set; }

        public Logger Logger { get; }

        public bool IsDisabled
        {
            get => isDisabled_ ?? visualGridRunner_.IsDisabled;
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

        public string FullAgentId
        {
            get
            {
                return (AgentId == null) ? BaseAgentId : $"{AgentId} [{BaseAgentId}]";
            }
        }

        public BatchInfo Batch
        {
            get => Config_.Batch ?? new BatchInfo();
            set => Config_.SetBatch(value);
        }

        public bool IsEyesClosed()
        {
            Logger.Verbose($"enter - {nameof(testList_)} has {0} tests", testList_.Count);
            foreach (RunningTest runningTest in testList_)
            {
                Logger.Verbose("is test {0} closed? {1}", runningTest.TestId, runningTest.IsTestClose);
                if (!runningTest.IsTestClose)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsOpen { get { return !IsEyesClosed(); } }

        public ICollection<TestResultContainer> TestResults { get; } = new List<TestResultContainer>();

        internal IEyesConnectorFactory EyesConnectorFactory { get; set; } = new EyesConnectorFactory();

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

            Logger.Log("Agent = {0}", FullAgentId);
            Logger.Verbose(".NET Framework = {0}", Environment.Version);

            ArgumentGuard.NotNull(webDriver, nameof(webDriver));

            string apiKey = ApiKey;
            EyesBase.ValidateAPIKey(apiKey);

            ArgumentGuard.NotEmpty(Config_.AppName, "appName");
            ArgumentGuard.NotEmpty(Config_.TestName, "testName");

            InitDriver(webDriver);

            string uaString = driver_.GetUserAgent();
            if (uaString != null)
            {
                Logger.Verbose("User-Agent: {0}", uaString);
                userAgent_ = UserAgent.ParseUserAgentString(uaString, true);
            }

            EnsureViewportSize_();

            closeFutures_.Clear();

            if (Config_.Batch == null)
            {
                Config_.SetBatch(Batch);
            }

            Logger.Verbose("getting all browsers info...");
            List<RenderBrowserInfo> browserInfoList = Config_.GetBrowsersInfo();
            if (browserInfoList.Count == 0)
            {
                DesktopBrowserInfo desktopBrowserInfo = new DesktopBrowserInfo(viewportSize_);
                browserInfoList.Add(new RenderBrowserInfo(desktopBrowserInfo));
            }

            configAtOpen_ = GetConfigClone_();

            Logger.Verbose("creating test descriptors for each browser info...");
            foreach (RenderBrowserInfo browserInfo in browserInfoList)
            {
                Logger.Verbose("creating test descriptor");
                RunningTest test = new RunningTest(CreateEyesConnector_(browserInfo, apiKey), browserInfo, Logger, testListener_, url_);
                testList_.Add(test);
            }

            Logger.Verbose("opening {0} tests...", testList_.Count);
            visualGridRunner_.Open(this, renderingInfo_);
            Logger.Verbose("done");
            return driver_ ?? webDriver;
        }

        internal IUfgConnector CreateEyesConnector_(RenderBrowserInfo browserInfo, string apiKey)
        {
            Logger.Verbose("creating eyes server connector using {0}", EyesConnectorFactory.GetType().Name);
            IUfgConnector eyesConnector = EyesConnectorFactory.CreateNewEyesConnector(
                Logger, browserInfo, (Applitools.Configuration)configAtOpen_);

            eyesConnector.SetLogHandler(Logger.GetILogHandler());
            eyesConnector.Proxy = Proxy;
            eyesConnector.Batch = Batch;
            eyesConnector.IsDisabled = IsDisabled;

            string serverUri = ServerUrl;
            if (serverUri != null)
            {
                eyesConnector.ServerUrl = serverUri;
            }

            eyesConnector.ApiKey = apiKey;

            if (renderingInfo_ == null)
            {
                Logger.Verbose("initializing rendering info...");
                renderingInfo_ = eyesConnector.GetRenderingInfo();
            }
            eyesConnector.SetRenderInfo(renderingInfo_);

            foreach (KeyValuePair<string, string> kvp in properties_)
            {
                eyesConnector.AddProperty(kvp.Key, kvp.Value);
            }
            properties_.Clear();

            eyesConnector_ = eyesConnector;
            return eyesConnector;
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
                driver_ = new EyesWebDriver(Logger, null, remoteDriver);
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
            List<Task<TestResultContainer>> tasks = AbortAndCollectTasks_();
            return ParseCloseTasks_(tasks, false);
        }

        public void AbortAsync()
        {
            Logger.Verbose("enter");
            AbortAndCollectTasks_();
            Logger.Verbose("exit");
        }

        private List<Task<TestResultContainer>> AbortAndCollectTasks_()
        {
            Logger.Verbose("enter");
            List<Task<TestResultContainer>> tasks = new List<Task<TestResultContainer>>();
            foreach (RunningTest runningTest in testList_)
            {
                Task<TestResultContainer> task = runningTest.AbortIfNotClosed();
                tasks.Add(task);
            }
            Logger.Verbose("exit");
            return tasks;
        }

        private void Abort_(Exception e)
        {
            foreach (RunningTest runningTest in testList_)
            {
                runningTest.Abort(e);
            }
        }

        private IList<VisualGridSelector[]> GetRegionsXPaths_(ICheckSettings checkSettings)
        {
            List<VisualGridSelector[]> result = new List<VisualGridSelector[]>();
            ICheckSettingsInternal csInternal = (ICheckSettingsInternal)checkSettings;
            IList<Tuple<IWebElement, object>>[] elementLists = CollectSeleniumRegions_(csInternal);

            CheckState state = ((ISeleniumCheckTarget)csInternal).State;
            if (state.FrameToSwitchTo != null)
            {
                driver_.SwitchTo().Frame(state.FrameToSwitchTo);
            }

            int i;
            for (i = 0; i < elementLists.Length - 1; ++i)
            {
                IList<Tuple<IWebElement, object>> elementsList = elementLists[i];
                GetRegionsXPaths_(result, elementsList);
            }

            if (state.FrameToSwitchTo != null)
            {
                driver_.SwitchTo().ParentFrame();
            }

            GetRegionsXPaths_(result, elementLists[i]);

            return result;
        }

        private void GetRegionsXPaths_(List<VisualGridSelector[]> result, IList<Tuple<IWebElement, object>> elementsList)
        {
            List<VisualGridSelector> xpaths = new List<VisualGridSelector>();
            foreach (Tuple<IWebElement, object> element in elementsList)
            {
                if (element.Item1 == null) continue;
                string xpath = (string)jsExecutor_.ExecuteScript(GET_ELEMENT_XPATH_JS, element.Item1);
                xpaths.Add(new VisualGridSelector(xpath, element.Item2));
            }
            result.Add(xpaths.ToArray());
        }

        private IList<Tuple<IWebElement, object>>[] CollectSeleniumRegions_(ICheckSettingsInternal csInternal)
        {
            CheckState state = ((ISeleniumCheckTarget)csInternal).State;
            if (state.FrameToSwitchTo != null)
            {
                driver_.SwitchTo().Frame(state.FrameToSwitchTo);
            }

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

            if (state.FrameToSwitchTo != null)
            {
                driver_.SwitchTo().ParentFrame();
            }

            IWebElement targetElement = ((ISeleniumCheckTarget)csInternal).GetTargetElement();
            if (targetElement == null)
            {
                By targetSelector = ((ISeleniumCheckTarget)csInternal).GetTargetSelector();
                if (targetSelector != null)
                {
                    targetElement = webDriver_.FindElement(targetSelector);
                }
            }

            Tuple<IWebElement, object> targetTuple = new Tuple<IWebElement, object>(targetElement, "target");
            List<Tuple<IWebElement, object>> targetElementList = new List<Tuple<IWebElement, object>>() { targetTuple };

            return new IList<Tuple<IWebElement, object>>[] { ignoreElements, layoutElements, strictElements, contentElements, floatingElements, accessibilityElements, targetElementList };
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
                Check(checkSetting);
            }
        }

        public void Check(ICheckSettings checkSettings)
        {
            ArgumentGuard.IsValidState(IsOpen, "Eyes not open");
            if (!ValidateEyes_()) return;

            Logger.Verbose("enter (#{0})", GetHashCode());

            try
            {
                object logMessage = visualGridRunner_.GetConcurrencyLog();
                if (logMessage != null)
                {
                    NetworkLogHandler.SendEvent(((EyesBase)eyesConnector_).ServerConnector, TraceLevel.Notice, logMessage);
                }
            }
            catch (JsonException e)
            {
                Logger.Log("Error: {0}", e);
            }

            AddOpenTaskToAllRunningTest_();

            List<VisualGridTask> checkTasks = new List<VisualGridTask>();

            ISeleniumCheckTarget seleniumCheckTarget = (ISeleniumCheckTarget)checkSettings;
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;

            CheckState state = new CheckState();
            state.StitchContent = checkSettingsInternal.GetStitchContent() ?? Config_.IsForceFullPageScreenshot ?? true;
            seleniumCheckTarget.State = state;
            ((SeleniumCheckSettings)checkSettings).SanitizeSettings(Logger, driver_, state);

            try
            {
                FrameChain originalFC = driver_.GetFrameChain().Clone();
                EyesWebDriverTargetLocator switchTo = ((EyesWebDriverTargetLocator)driver_.SwitchTo());

                IList<FrameLocator> frameLocators = seleniumCheckTarget.GetFrameChain();
                int switchedToCount = frameLocators.Count;
                foreach (FrameLocator locator in frameLocators)
                {
                    IWebElement frameElement = EyesSeleniumUtils.FindFrameByFrameCheckTarget(locator, driver_);
                    switchTo.Frame(frameElement);
                }

                if (switchedToCount > 0 && !(checkSettingsInternal.GetStitchContent() ?? Config_.IsForceFullPageScreenshot ?? true))
                {
                    FrameChain frameChain = driver_.GetFrameChain().Clone();
                    Frame frame = frameChain.Pop();
                    checkSettings = ((SeleniumCheckSettings)checkSettings).Region(frame.Reference);
                    seleniumCheckTarget = checkSettings as ISeleniumCheckTarget;
                    checkSettingsInternal = checkSettings as ICheckSettingsInternal;
                    switchTo.ParentFrame();
                }

                FrameData dom = CaptureDomSnapshot_(switchTo, userAgent_, configAtOpen_, visualGridRunner_, driver_, Logger);

                //string scriptResult = captureStatus.Value;
                IList<VisualGridSelector[]> regionSelectors = GetRegionsXPaths_(checkSettings);

                //RGridResource gridResource = new RGridResource(new Uri(url_), "application/json", Encoding.UTF8.GetBytes(scriptResult), Logger, "");
                debugResourceWriter_.Write(dom);
                //Logger.Verbose("Done collecting DOM.");

                TrySetTargetSelector_((SeleniumCheckSettings)checkSettings);

                checkSettings = UpdateCheckSettings_(checkSettings);
                List<RunningTest> filteredTests = CollectTestsForCheck_(Logger, testList_);

                Configuration configClone = GetConfigClone_();

                Logger.Verbose("eyesConnector_.Type: {0}", eyesConnector_.GetType().Name);

                string source = driver_.Url;
                foreach (RunningTest test in filteredTests)
                {
                    VisualGridTask checkTask = test.Check(configClone, checkSettings, regionSelectors, source);
                    checkTasks.Add(checkTask);
                }

                visualGridRunner_.Check(checkSettings, debugResourceWriter_, dom, regionSelectors,
                        eyesConnector_, userAgent_, checkTasks,
                        new VisualGridRunner.RenderListener());

                switchTo.Frames(originalFC);
            }
            catch (Exception ex)
            {
                Abort_(ex);
                foreach (RunningTest test in testList_)
                {
                    test.SetTestInExceptionMode(ex);
                }
                Logger.Log("Error: {0}", ex);
            }
        }

        internal static List<RunningTest> CollectTestsForCheck_(Logger logger, List<RunningTest> tests)
        {
            List<RunningTest> filteredTests = new List<RunningTest>();
            foreach (RunningTest test in tests)
            {
                TaskType? lastTaskType = test.TaskList.LastOrDefault()?.TaskType;
                logger.Debug("task type: <{0}>", lastTaskType);

                bool testIsOpenAndNotClosed = lastTaskType == null && test.IsOpenTaskIssued && !test.IsCloseTaskIssued;
                bool lastTaskIsNotAClosingTask = lastTaskType != null && lastTaskType != TaskType.Close && lastTaskType != TaskType.Abort;

                // We are interested in tests which should be opened or are open.
                if (testIsOpenAndNotClosed || lastTaskIsNotAClosingTask)
                {
                    filteredTests.Add(test);
                }
            }

            return filteredTests;
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

            MatchLevel? matchLevel = checkSettingsInternal.GetMatchLevel();
            bool? fully = checkSettingsInternal.GetStitchContent();
            bool? sendDom = checkSettingsInternal.GetSendDom();
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
            IConfiguration config, IVisualGridRunner runner, EyesWebDriver driver, Logger logger)
        {
            string domScript = userAgent.IsInternetExplorer ? PROCESS_PAGE_FOR_IE : PROCESS_PAGE;
            string pollingScript = userAgent.IsInternetExplorer ? POLL_RESULT_FOR_IE : POLL_RESULT;


            int chunkByteLength = userAgent.IsiOS ? 10 * MB : 256 * MB;
            object arguments = new
            {
                serializeResources = true,
                skipResources = runner.CachedBlobsURLs.Keys,
                dontFetchResources = config.DisableBrowserFetching,
                chunkByteLength
            };

            object pollingArguments = new { chunkByteLength };

            string result = EyesSeleniumUtils.RunDomScript(logger, driver, domScript, arguments, pollingArguments, pollingScript);
            FrameData frameData = JsonConvert.DeserializeObject<FrameData>(result);
            AnalyzeFrameData_(frameData, userAgent, config, runner, switchTo, driver, logger);
            return frameData;
        }

        private static void AnalyzeFrameData_(FrameData frameData, UserAgent userAgent, IConfiguration config,
            IVisualGridRunner runner, EyesWebDriverTargetLocator switchTo, EyesWebDriver driver, Logger logger)
        {
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
                    FrameData result = CaptureDomSnapshot_(switchTo, userAgent, config, runner, driver, logger);
                    frameData.Frames.Add(result);
                    frameData.Cdt[crossFrame.Index].Attributes.Add(new AttributeData("data-applitools-src", result.Url.AbsoluteUri));
                }
                catch (Exception e)
                {
                    logger.Log("Failed finding cross frame with selector {0}. Reason: {1}", crossFrame.Selector, e);
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
                    AnalyzeFrameData_(frame, userAgent, config, runner, switchTo, driver, logger);
                }
                catch (Exception e)
                {
                    logger.Log("Failed finding cross frame with selector {0}. Reason: {1}", frame.Selector, e);
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

        public ICollection<Task<TestResultContainer>> Close()
        {
            if (!ValidateEyes_()) return new Task<TestResultContainer>[0];
            closeFutures_ = CloseAndReturnResults(false);
            return closeFutures_;
        }

        public TestResults Close(bool throwEx)
        {
            Logger.Verbose("enter. throwEx: {0}", throwEx);
            ICollection<Task<TestResultContainer>> close = Close();
            return ParseCloseTasks_(close, throwEx);
        }

        private TestResults ParseCloseTasks_(ICollection<Task<TestResultContainer>> close, bool throwEx)
        {
            Logger.Verbose("enter");
            if (close != null && close.Count > 0)
            {
                Logger.Verbose("closing {0} tasks.", close.Count);
                TestResultContainer errorResult = null;
                TestResultContainer firstResult = null;
                try
                {
                    foreach (Task<TestResultContainer> closeFuture in close)
                    {
                        if (closeFuture == null)
                        {
                            Logger.Log("Error: closeFuture is null!");
                            continue;
                        }
                        Task result = Task.WhenAny(closeFuture, Task.Delay(TimeSpan.FromSeconds(30))).Result;
                        if (result != closeFuture)
                        {
                            throw new Exception("timeout");

                        }
                        TestResultContainer testResultContainer = closeFuture.Result;
                        if (firstResult == null)
                        {
                            firstResult = testResultContainer;
                        }

                        Exception error = testResultContainer.Exception;
                        if (error != null && errorResult == null)
                        {
                            errorResult = testResultContainer;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Log("Error: {0}", e);
                }

                if (errorResult != null)
                {
                    if (throwEx)
                    {
                        throw errorResult.Exception;
                    }
                    Logger.Verbose("returning errorResult.TestResults: {0}", errorResult.TestResults);
                    return errorResult.TestResults;
                }
                else
                { // returning the first result
                    if (firstResult != null)
                    {
                        Logger.Verbose("returning firstResult.TestResults: {0}", firstResult.TestResults);
                        return firstResult.TestResults;
                    }
                }
            }
            Logger.Verbose("returning null");
            return null;
        }

        private ICollection<Task<TestResultContainer>> CloseAndReturnResults(bool throwEx)
        {
            if (!ValidateEyes_()) return new Task<TestResultContainer>[0];

            Exception exception = null;
            Logger.Verbose("enter {0}", Batch);

            try
            {
                ICollection<Task<TestResultContainer>> futureList = CloseAsync(throwEx);
                visualGridRunner_.Close(this);
                foreach (Task<TestResultContainer> future in futureList)
                {
                    Task result = Task.WhenAny(future, Task.Delay(TimeSpan.FromSeconds(30))).Result;
                    if (result != future)
                    {
                        if (exception == null)
                        {
                            exception = new Exception("timeout");
                        }
                    }
                    else
                    {
                        TestResultContainer testResultContainer = future.Result;
                        if (exception == null && testResultContainer.Exception != null)
                        {
                            exception = testResultContainer.Exception;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log("Error: {0}", e);
                if (exception == null)
                {
                    exception = e;
                }
            }

            //testList_.Clear();
            //testsInCloseProcess_.Clear();
            //properties_.Clear();

            if (throwEx)
            {
                throw exception;
            }
            return closeFutures_;
        }

        internal ICollection<Task<TestResultContainer>> CloseAsync(bool throwEx)
        {
            if (!ValidateEyes_()) return new Task<TestResultContainer>[0];

            Configuration configClone = GetConfigClone_();

            ICollection<Task<TestResultContainer>> futureList = AddCloseTasks_(configClone, throwEx);
            foreach (Task<TestResultContainer> future in futureList)
            {
                closeFutures_.Add(future);
            }
            Logger.Verbose("futures_: {0}", PrintAllFutures());
            visualGridRunner_.Close(this);
            return futureList;
        }

        private ICollection<Task<TestResultContainer>> AddCloseTasks_(Configuration config, bool throwEx)
        {
            HashSet<Task<TestResultContainer>> futures = new HashSet<Task<TestResultContainer>>();
            lock (testList_)
            {
                foreach (RunningTest runningTest in testList_)
                {
                    Logger.Verbose("running test name: {0}", Config_.TestName);
                    Logger.Verbose("is current running test open: {0}", runningTest.IsTestOpen);
                    Logger.Verbose("is current running test ready to close: {0}", runningTest.IsTestReadyToClose);
                    Logger.Verbose("is current running test closed: {0}", runningTest.IsTestClose);
                    Logger.Verbose("closing current running test");
                    Task<TestResultContainer> closeFuture = runningTest.Close(config, throwEx);
                    Logger.Verbose("adding closeFuture to futureList. closeFuture == null? {0}", closeFuture == null);
                    futures.Add(closeFuture);
                }
            }
            return futures;
        }

        void IVisualGridEyes.SetListener(EyesListener listener)
        {
            listener_ = listener;
        }

        RunningTest IVisualGridEyes.GetNextTestToClose()
        {
            Logger.Verbose("locking 'testsInCloseProcess_'. Count: {0} (object #{1})", testList_.Count, GetHashCode());
            lock (testsInCloseProcess_)
            {
                foreach (RunningTest runningTest in testList_)
                {
                    if (runningTest.IsTestClose) continue;

                    bool isTestReadyToClose = runningTest.IsTestReadyToClose;
                    bool containedInList = testsInCloseProcess_.Contains(runningTest);

                    if (isTestReadyToClose && !containedInList)
                    {
                        testsInCloseProcess_.Add(runningTest);
                        Logger.Verbose("found test to close: #{0}. lock on 'testsInCloseProcess_' released", runningTest.GetHashCode());
                        return runningTest;
                    }
                    else
                    {
                        Logger.Verbose("runningTest: IsTestReadyToClose: {0} ; contained in list: {1} ; inner tasks: {2}",
                            isTestReadyToClose, containedInList, runningTest.TaskList.Count);
                    }
                }
            }
            Logger.Verbose("lock on 'testsInCloseProcess_' released");
            return null;
        }

        ScoreTask IVisualGridEyes.GetBestScoreTaskForOpen()
        {
            int bestMark = -1;
            ScoreTask currentBest = null;
            lock (testList_)
            {
                foreach (RunningTest runningTest in testList_)
                {
                    ScoreTask currentScoreTask = runningTest.GetScoreTaskObjectByType(TaskType.Open);
                    if (currentScoreTask == null) continue;

                    if (bestMark < currentScoreTask.Score)
                    {
                        bestMark = currentScoreTask.Score;
                        currentBest = currentScoreTask;
                    }
                }
            }
            return currentBest;
        }

        ScoreTask IVisualGridEyes.GetBestScoreTaskForCheck()
        {
            int bestScore = -1;

            ScoreTask currentBest = null;
            Logger.Verbose("enter - {0} elements in {1}", testList_.Count, nameof(testList_));
            foreach (RunningTest runningTest in testList_)
            {
                List<VisualGridTask> taskList = runningTest.TaskList;

                VisualGridTask task;
                //Logger.Verbose("locking taskList");
                try
                {
                    Monitor.Enter(taskList);
                    if (taskList.Count == 0) continue;

                    task = taskList[0];
                    if (!runningTest.IsTestOpen || task.TaskType != TaskType.Check || !task.IsTaskReadyToCheck)
                        continue;
                }
                finally
                {
                    //Logger.Verbose("releasing taskList");
                    Monitor.Exit(taskList);
                }

                ScoreTask scoreTask = runningTest.GetScoreTaskObjectByType(TaskType.Check);

                if (scoreTask == null) continue;

                if (bestScore < scoreTask.Score)
                {
                    currentBest = scoreTask;
                    bestScore = scoreTask.Score;
                }
            }
            return currentBest;
        }

        private List<VisualGridTask> AddOpenTaskToAllRunningTest_()
        {
            Logger.Verbose("enter");
            List<VisualGridTask> tasks = new List<VisualGridTask>();
            foreach (RunningTest runningTest in testList_)
            {
                if (!runningTest.IsOpenTaskIssued)
                {
                    VisualGridTask task = runningTest.Open((Configuration)configAtOpen_);
                    tasks.Add(task);
                }
            }
            Logger.Verbose("exit");
            return tasks;
        }

        public ICollection<RunningTest> GetAllRunningTests()
        {
            return testList_;
        }

        public void AddProperty(string name, string value)
        {
            if (eyesConnector_ == null)
            {
                properties_.Add(name, value);
            }
            else
            {
                eyesConnector_.AddProperty(name, value);
            }
        }

        public void ClearProperties()
        {
            if (eyesConnector_ == null)
            {
                properties_.Clear();
            }
            else
            {
                eyesConnector_.ClearProperties();
            }
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
            if (!visualGridRunner_.IsServicesOn)
            {
                Logger.Verbose("WARNING! Invalid Operation - visualGridRunner.getAllTestResults already called!");
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
            return testList_[0].GetBatchCloser();
        }

        internal delegate void AfterServerConcurrencyLimitReachedQueriedDelegate(bool value);
        internal event AfterServerConcurrencyLimitReachedQueriedDelegate AfterServerConcurrencyLimitReachedQueried;

        bool IVisualGridEyes.IsServerConcurrencyLimitReached()
        {
            bool result = testList_.Any(t => t.IsServerConcurrencyLimitReached);
            AfterServerConcurrencyLimitReachedQueried?.Invoke(result);
            return result;
        }
    }
}
