using Applitools.Utils;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public class MatchWindowTask
    {
        public static ImageMatchSettings CreateImageMatchSettings(ICheckSettingsInternal checkSettingsInternal, object visualGridRunningTest)
        {
            throw new NotImplementedException();
        }

        public static void CollectRegions(ImageMatchSettings imageMatchSettings, List<Trigger> userInputs, List<IRegion> regions, IList<VisualGridSelector[]> regionSelectors, IList<VGUserAction> userActions, Location location)
        {
            throw new NotImplementedException();
        }

        public static void CollectRegions(ImageMatchSettings imageMatchSettings, ICheckSettingsInternal checkSettingsInternal)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RunningTest : EyesBase, IRunningTest
    {
        protected Exception exception_;
        protected string agentRunId_;
        private bool? isAbortIssued_ = null;
        private bool inOpenProcess_ = false;
        private bool startedCloseProcess_ = false;

        internal bool isCloseTaskIssued_;

        public static readonly int PARALLEL_STEPS_LIMIT = 1;

        protected virtual Configuration Configuration { get; }
        protected ServerConnector ServerConnector { get; }

        protected internal  void OpenCompleted(RunningSession result)
        {
            inOpenProcess_ = false;
           // base.OpenCompleted(result);
        }

        protected virtual string BaseAgentId { get; }

        protected internal virtual object GetEnvironment_()
        {
            return null;
        }

        protected virtual string GetBaselineEnvName()
        {
            throw new NotImplementedException();
        }

        public virtual IDictionary<string, IRunningTest> GetAllTests()
        {
            return new Dictionary<string, IRunningTest>() { { TestId, this } };
        }
        protected FileVersionInfo GetActualAssemblyVersionInfo()
        {
            throw new NotImplementedException();
        }

        protected void ValidateResult_(string getName, MatchResult matchResult)
        {
            throw new NotImplementedException();
        }

        protected MatchWindowData PrepareForMatch(ICheckSettingsInternal checkSettingsInternal, List<Trigger> userInputs, AppOutput appOutput, string tag, bool b, ImageMatchSettings imageMatchSettings, string renderId, string checkTaskSource)
        {
            throw new NotImplementedException();
        }

        protected virtual void SetConfigImpl(IConfiguration configuration)
        {

        }

        protected virtual EyesScreenshot GetScreenshot(Rectangle? region, ICheckSettingsInternal checkSettingsInternal)
        {
            return null;
        }

        protected virtual string GetTitle()
        {
            return null;
        }

        protected virtual Size GetViewportSize()
        {
            return Size.Empty;
        }

        protected virtual void SetViewportSize(RectangleSize size)
        {
            Logger.Log(TraceLevel.Warn, TestId, Stage.Open);
        }

        internal void OpenFailed(Exception e)
        {
            inOpenProcess_ = false;
            SetTestInExceptionMode(e);
        }

        protected internal abstract MatchWindowData PrepareForMatch(ICheckTask checkTask);

        protected internal abstract ICheckTask IssueCheck(ICheckSettings checkSettings, IList<VisualGridSelector[]> regionSelectors,
            string source, IList<IUserAction> userInputs);

        protected internal abstract void CheckCompleted(ICheckTask checkTask, MatchResult matchResult);

        protected RunningTest(ClassicRunner runner)// : base(runner)
            : base(runner)
        {
            BrowserInfo = null;
        }

        protected RunningTest(RenderBrowserInfo browserInfo, Logger logger, IServerConnector serverConnector)
            //: base(logger, serverConnector)
            : base(new ClassicRunner())
        {
            BrowserInfo = browserInfo;
        }

        protected RunningTest(ClassicRunner runner, IServerConnectorFactory serverConnectorFactory, Logger logger = null)
            //: base(serverConnectorFactory, runner, logger)
            : base(runner)
        {
        }

        protected  string GetInferredEnvironment()
        {
            return null;
        }

        public virtual bool IsTestReadyToClose => GetIsTestReadyToClose();

        protected bool GetIsTestReadyToClose()
        {
            return !inOpenProcess_ && isAbortIssued_ != null && !startedCloseProcess_;
        }

        public override TestResults Abort()
        {
            throw new NotImplementedException();
        }

        public bool IsCloseTaskIssued { get => isAbortIssued_ != null; }

        private RenderBrowserInfo browserInfo_;
        private TestResultContainer testResultContainer_;

        public RenderBrowserInfo BrowserInfo
        {
            get => browserInfo_;
            set
            {
                ObsoleteAttribute obsoleteAttr = value?.BrowserType.GetAttribute<ObsoleteAttribute>();
                if (obsoleteAttr != null)
                {
                    Console.WriteLine("WARNING: " + obsoleteAttr.Message);
                }
                browserInfo_ = value;
            }
        }

        protected internal SessionStartInfo PrepareForOpen()
        {
            inOpenProcess_ = true;
            throw new NotImplementedException();
        }

        public bool IsTestAborted => isAbortIssued_ ?? false;

        bool IRunningTest.IsCompleted => GetIsCompleted();
        public string TestId { get; set; }

        private bool GetIsCompleted()
        {
            throw new NotImplementedException();
        }

        protected internal void IssueClose()
        {
            if (IsCloseTaskIssued)
            {
                return;
            }

            isAbortIssued_ = false;
        }

        protected internal virtual void IssueAbort(Exception exception, bool forceAbort)
        {
            if (IsCloseTaskIssued && !forceAbort)
            {
                return;
            }

            isAbortIssued_ = true;
            if (exception_ == null)
            {
                exception_ = exception;
            }
        }

        protected internal void CloseCompleted(TestResults testResults)
        {
            startedCloseProcess_ = true;
            if (!IsTestAborted)
            {
                try
                {
                    LogSessionResultsAndThrowException(true, testResults);
                }
                catch (Exception e)
                {
                    //CommonUtils.LogExceptionStackTrace(Logger, Stage.General, e, TestId);
                    exception_ = e;
                }
            }

            testResultContainer_ = new TestResultContainer(testResults, BrowserInfo, exception_);
        }

        private void LogSessionResultsAndThrowException(bool b, TestResults testResults)
        {
            throw new NotImplementedException();
        }

        protected internal void CloseFailed(Exception e)
        {
            startedCloseProcess_ = true;
            if (exception_ == null)
            {
                exception_ = e;
            }

            testResultContainer_ = new TestResultContainer(null, BrowserInfo, exception_);
        }

        protected internal SessionStopInfo PrepareStopSession(bool isAborted)
        {
            startedCloseProcess_ = true;

            throw new NotImplementedException();
        }

        protected internal void SetTestInExceptionMode(Exception e)
        {
            throw new NotImplementedException();
        }

        public TestResultContainer GetTestResultContainer() { return testResultContainer_; }
        public void CloseBatch(string batchId)
        {
            throw new NotImplementedException();
        }
    }
}