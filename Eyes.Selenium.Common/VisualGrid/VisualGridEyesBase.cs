using Applitools.Fluent;
using Applitools.Selenium.Common;
using Applitools.Selenium.Fluent;
using Applitools.Ufg;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Selenium.VisualGrid
{
    public abstract class VisualGridEyesBase<IWebElement, IWebDriver> : IVisualGridEyes, IVisualGridEyesInternal
    {
        protected static readonly int MB = 1024 * 1024;

        protected static readonly string GET_ELEMENT_XPATH_JS =
            "const atName='data-applitools-element-id'; var el=arguments[0];" +
            "if (el.hasAttribute(atName)) { var id = el.getAttribute(atName) } " +
            "else { window.APPLITOOLS_ELEMENT_ID = window.APPLITOOLS_ELEMENT_ID || 0; " +
            " window.APPLITOOLS_ELEMENT_ID++; var id = window.APPLITOOLS_ELEMENT_ID;" +
            " el.setAttribute(atName, id); }" +
            "return '//*[@'+atName+'=\"'+id+'\"]';";

        internal readonly VisualGridRunner runner_;
        protected readonly Dictionary<string, IRunningTest> testList_ = new Dictionary<string, IRunningTest>();
        protected readonly List<RunningTest> testsInCloseProcess_ = new List<RunningTest>();
        protected ICollection<Task<TestResultContainer>> closeFutures_ = new HashSet<Task<TestResultContainer>>();
        protected ISeleniumConfigurationProvider configProvider_;
        internal Ufg.IDebugResourceWriter debugResourceWriter_;
        protected string url_;
        protected bool? isDisabled_;
        protected IConfiguration configAtOpen_;
        internal UserAgent userAgent_;
        protected readonly Dictionary<string, string> properties_ = new Dictionary<string, string>();
        protected RectangleSize viewportSize_;
        protected bool isOpen_;
        protected readonly string eyesId_ = Guid.NewGuid().ToString();
        protected IWebDriver webDriver_;
        protected IEyesJsExecutor jsExecutor_;

        public VisualGridEyesBase(ISeleniumConfigurationProvider configurationProvider, VisualGridRunner visualGridRunner)
        {
            ArgumentGuard.NotNull(visualGridRunner, nameof(visualGridRunner));
            if (visualGridRunner.GetAllTestResultsAlreadyCalled)
            {
                throw new InvalidOperationException("Runner already returned its results");
            }
            runner_ = visualGridRunner;
            configProvider_ = configurationProvider;
            Logger = visualGridRunner.Logger;

            IDebugResourceWriter drw = runner_.DebugResourceWriter;
            Ufg.IDebugResourceWriter ufgDrw = Applitools.VisualGrid.Utils.ConvertDebugResourceWriter(drw);
            debugResourceWriter_ = ufgDrw;
        }

        public Logger Logger { get; }


        protected Configuration Config_ { get => configProvider_.GetConfiguration(); }

        protected static string GetBaseAgentId()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"Eyes.Selenium4.VisualGrid.DotNet/{versionInfo.ProductVersion}";
        }

        public string AgentId
        {
            get => Config_.AgentId;
            set => Config_.SetAgentId(value);
        }

        public string ApiKey
        {
            get => runner_.ApiKey ?? Config_.ApiKey;
            set { runner_.ApiKey = value; Config_.ApiKey = value; }
        }

        public string ServerUrl
        {
            get => runner_.ServerUrl ?? Config_.ServerUrl;
            set { runner_.ServerUrl = value; Config_.ServerUrl = value; }
        }

        public ProxySettings Proxy
        {
            get => runner_.Proxy ?? Config_.Proxy;
            set { runner_.Proxy = value; Config_.Proxy = value; }
        }

        public bool IsDisabled
        {
            get => isDisabled_ ?? runner_.IsDisabled;
            set => isDisabled_ = value;
        }

        public string FullAgentId => (AgentId == null) ? BaseAgentId : $"{AgentId} [{BaseAgentId}]";
        protected abstract string BaseAgentId { get; }

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

        public ICollection<TestResultContainer> TestResults { get; } = new List<TestResultContainer>();


        public bool IsOpen => !IsEyesClosed();


        public bool IsCompleted()
        {
            return GetAllTestResults() != null;
        }

        bool IVisualGridEyesInternal.IsCloseIssued => !isOpen_;

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
        internal abstract void CheckImpl_(ICheckSettings checkSettings);

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

        public TestResults Close(bool throwEx = true)
        {
            CloseAsync();
            return WaitForEyesToFinish_(throwEx);
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

        public IBatchCloser GetBatchCloser()
        {
            return testList_.Values.FirstOrDefault();
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

        protected bool ValidateEyes_()
        {
            if (IsDisabled)
            {
                Logger.Verbose("WARNING! Invalid Operation - Eyes Disabled!");
                return false;
            }
            return true;
        }


        public void SetLogHandler(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
        }

        public void AddProperty(string name, string value)
        {
            properties_.Add(name, value);
        }

        public void ClearProperties()
        {
            properties_.Clear();
        }

        internal ICheckSettings UpdateCheckSettings_(ICheckSettings checkSettings)
        {
            ICheckSettingsInternal checkSettingsInternal = (ICheckSettingsInternal)checkSettings;
            ISeleniumCheckTargetCommon seleniumCheckTarget = (ISeleniumCheckTargetCommon)checkSettings;

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

        protected Configuration GetConfigClone()
        {
            return new Configuration(configProvider_.GetConfiguration());
        }

        internal Dictionary<int, List<RunningTest>> MapRunningTestsToRequiredBrowserWidth_(ISeleniumCheckTargetCommon seleniumCheckSettings)
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
                layoutBreakpoints = GetConfigClone().LayoutBreakpoints;
                isLayoutBreakpointsEnabled = GetConfigClone().LayoutBreakpointsEnabled;
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

        #region collect elements xpath

        protected IList<VisualGridSelector[]> GetRegionsXPaths(ICheckSettings checkSettings)
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
                elements.Add(new Tuple<IWebElement, object>(element, CreateSimpleRegionByElement(element)));
            }
            return elements;
        }

        protected abstract object CreateSimpleRegionByElement(IWebElement element);

        private IList<Tuple<IWebElement, object>> GetElementsFromRegions_(IList regionsProvider)
        {
            List<Tuple<IWebElement, object>> elements = new List<Tuple<IWebElement, object>>();
            foreach (object getRegions in regionsProvider)
            {
                if (getRegions is IGetSeleniumRegion<IWebElement, IWebDriver> getSeleniumRegion)
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

        #endregion

        public override string ToString()
        {
            return $"{GetType().Name} (#{GetHashCode()}) - close future count: {closeFutures_.Count}";
        }
    }
}
