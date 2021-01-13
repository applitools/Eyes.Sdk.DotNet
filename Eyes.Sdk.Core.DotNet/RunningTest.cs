using Applitools.Utils;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;

namespace Applitools
{
    public abstract class RunningTest : EyesBase, IRunningTest
    {
#pragma warning disable IDE0052 // Remove unread private members
#pragma warning disable CS0414 // Filed is assigned but its value is never used
        private bool isTestInExceptionMode_ = false;
        protected Exception exception_;
#pragma warning restore CS0414 // Filed is assigned but its value is never used
#pragma warning restore IDE0052 // Remove unread private members

        private bool? isAbortIssued_ = null;
        private bool inOpenProcess_ = false;
        private bool startedCloseProcess_ = false;

        internal bool isCloseTaskIssued_;
      
        public static readonly int PARALLEL_STEPS_LIMIT = 1;

        public override void OpenCompleted(RunningSession result)
        {
            inOpenProcess_ = false;
            base.OpenCompleted(result);
        }

        public void OpenFailed(Exception e)
        {
            inOpenProcess_ = false;
            SetTestInExceptionMode(e);
        }

        public abstract MatchWindowData PrepareForMatch(ICheckTask checkTask);

        public abstract ICheckTask IssueCheck(ICheckSettings checkSettings, IList<VisualGridSelector[]> regionSelectors, string source);

        public abstract void CheckCompleted(ICheckTask checkTask, MatchResult matchResult);


        protected RunningTest(ClassicRunner runner) : base(runner)
        {
            BrowserInfo = null;
        }

        protected RunningTest(RenderBrowserInfo browserInfo, Logger logger, IServerConnector serverConnector) 
            : base(logger, serverConnector)
        {
            BrowserInfo = browserInfo;
        }

        protected RunningTest(ClassicRunner runner, IServerConnectorFactory serverConnectorFactory, Logger logger = null)
            :base(serverConnectorFactory, runner, logger)
        {
        }

        protected override string GetInferredEnvironment()
        {
            return null;
        }

        public virtual bool IsTestReadyToClose => !inOpenProcess_ && isAbortIssued_ != null && !startedCloseProcess_;

        public bool IsCloseTaskIssued { get => isAbortIssued_ != null; }

        private RenderBrowserInfo browserInfo_;

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

        public override SessionStartInfo PrepareForOpen()
        {
            inOpenProcess_ = true;
            return base.PrepareForOpen();
        }

        public bool IsTestAborted => isAbortIssued_ ?? false;
        
        public void IssueClose()
        {
            if (IsCloseTaskIssued)
            {
                return;
            }

            isAbortIssued_ = false;
        }

        public virtual void IssueAbort(Exception exception, bool forceAbort)
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

        public void CloseCompleted(TestResults testResults)
        {
            startedCloseProcess_ = true;
            if (!IsTestAborted)
            {
                try
                {
                    LogSessionResultsAndThrowException(Logger, true, testResults);
                }
                catch (Exception e)
                {
                    Logger.Log("Error: {0}", e);
                    exception_ = e;
                }
            }

            testResultContainer_ = new TestResultContainer(testResults, BrowserInfo, exception_);
        }

        public void CloseFailed(Exception e)
        {
            startedCloseProcess_ = true;
            if (exception_ == null)
            {
                exception_ = e;
            }

            testResultContainer_ = new TestResultContainer(null, BrowserInfo, exception_);
        }

        public override SessionStopInfo PrepareStopSession(bool isAborted)
        {
            startedCloseProcess_ = true;
            return base.PrepareStopSession(isAborted);
        }

        public void SetTestInExceptionMode(Exception e)
        {
            Logger.Log("Error: {0}", e);
            if (IsTestAborted)
            {
                return;
            }
            IssueAbort(e, true);
        }

        public string TestId { get; private set; } = Guid.NewGuid().ToString();

        public TestResultContainer GetTestResultContainer() { return testResultContainer_; }
    }
}