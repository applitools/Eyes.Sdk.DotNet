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

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            if (shouldThrowException && Exception != null)
            {
                throw Exception;
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
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(null, null);
            openService_.Operate(sessionStartInfo, listener);
            return listener.Get();
        }

        public MatchResult Check(MatchWindowData matchWindowData)
        {
            SyncTaskListener<bool?> listener = new SyncTaskListener<bool?>();
            checkService_.TryUploadImage(matchWindowData, new TaskListener(
                () => listener.OnComplete(true),
                e => listener.OnFail(e)
                ));
            bool? result = listener.Get();
            if (!result.HasValue || !result.Value)
            {
                throw new EyesException("Failed performing match with the server");
            }

            SyncTaskListener<MatchResult> matchListener = new SyncTaskListener<MatchResult>();
            checkService_.MatchWindow(matchWindowData, matchListener);
            return matchListener.Get();
        }


        public TestResults Close(SessionStopInfo sessionStopInfo)
        {
            SyncTaskListener<TestResults> listener = new SyncTaskListener<TestResults>();
            closeService_.Operate(sessionStopInfo, listener);
            return listener.Get();
        }
    }
}
