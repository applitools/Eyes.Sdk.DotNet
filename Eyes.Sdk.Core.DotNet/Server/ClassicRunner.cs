using System;
using System.Collections.Generic;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        private readonly OpenService openService_;
        private readonly CheckService checkService_;
        private readonly CloseService closeService_;

        private List<TestResults> allTestResult_ = new List<TestResults>();

        public EyesException Exception { get; set; }

        public ClassicRunner()
        {
            openService_ = new OpenService(Logger, ServerConnector, 1);
            checkService_ = new CheckService(Logger, ServerConnector);
            closeService_ = new CloseService(Logger, ServerConnector);
        }

        public void UpdateServerConnector(IServerConnector serverConnector)
        {
            ServerConnector = serverConnector;
            openService_.ServerConnector = serverConnector;
            checkService_.ServerConnector = serverConnector;
            closeService_.ServerConnector = serverConnector;
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            if (shouldThrowException && Exception != null)
            {
                throw new EyesException("Error", Exception);
            }
            List<TestResultContainer> result = new List<TestResultContainer>();
            foreach (TestResults testResults in allTestResult_)
            {
                result.Add(new TestResultContainer(testResults, null, null));
                EyesBase.LogSessionResultsAndThrowException(Logger, shouldThrowException, testResults);
            }

            return new TestResultsSummary(result);
        }

        public void AggregateResult(TestResults testResult)
        {
            allTestResult_.Add(testResult);
        }

        public RunningSession Open(SessionStartInfo sessionStartInfo)
        {
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(logger: Logger);
            openService_.Operate(sessionStartInfo, listener);
            return listener.Get();
        }

        public TestResults Close(SessionStopInfo sessionStopInfo)
        {
            SyncTaskListener<TestResults> listener = new SyncTaskListener<TestResults>(logger: Logger);
            closeService_.Operate(sessionStopInfo, listener);
            return listener.Get();
        }
    }
}
