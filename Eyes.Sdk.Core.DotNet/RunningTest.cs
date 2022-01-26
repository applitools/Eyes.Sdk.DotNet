using Applitools.Utils;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;

namespace Applitools
{
    public abstract class RunningTest : EyesBase, IRunningTest
    {
        protected Exception exception_;
        private bool? isAbortIssued_ = null;
        private bool inOpenProcess_ = false;
        private bool startedCloseProcess_ = false;

        internal bool isCloseTaskIssued_;

        public static readonly int PARALLEL_STEPS_LIMIT = 1;

        protected internal  void OpenCompleted(RunningSession result)
        {
            inOpenProcess_ = false;
           // base.OpenCompleted(result);
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
        {
            BrowserInfo = null;
        }

        protected RunningTest(RenderBrowserInfo browserInfo, Logger logger, IServerConnector serverConnector)
            //: base(logger, serverConnector)
        {
            BrowserInfo = browserInfo;
        }

        protected RunningTest(ClassicRunner runner, IServerConnectorFactory serverConnectorFactory, Logger logger = null)
            //: base(serverConnectorFactory, runner, logger)
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