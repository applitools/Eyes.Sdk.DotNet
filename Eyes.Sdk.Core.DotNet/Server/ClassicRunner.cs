using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        private readonly OpenService openService_;
        private readonly CheckService checkService_;
        private readonly CloseService closeService_;

        private List<TestResultContainer> allTestResult_ = new List<TestResultContainer>();

        private EyesBase eyes_;

        public EyesException Exception { get; set; }

        protected override IEnumerable<IEyesBase> GetAllEyes() => new EyesBase[] { eyes_ };
        internal void SetEyes(EyesBase eyes)
        {
            eyes_ = eyes;
        }

        public ClassicRunner(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
            openService_ = new OpenService(Logger, ServerConnector, 1);
            checkService_ = new CheckService(Logger, ServerConnector);
            closeService_ = new CloseService(Logger, ServerConnector);
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory)
        {
            ArgumentGuard.NotNull(serverConnectorFactory, nameof(serverConnectorFactory));
            Logger.SetLogHandler(logHandler);
            ServerConnectorFactory = serverConnectorFactory;
            ServerConnector = ServerConnectorFactory.CreateNewServerConnector(Logger, new Uri(ServerUrl));
            openService_ = new OpenService(Logger, ServerConnector, 1);
            checkService_ = new CheckService(Logger, ServerConnector);
            closeService_ = new CloseService(Logger, ServerConnector);
        }

        public ClassicRunner() : this(NullLogHandler.Instance) { }

        public void UpdateServerConnector(IServerConnector serverConnector)
        {
            ServerConnector = serverConnector;
            openService_.ServerConnector = serverConnector;
            checkService_.ServerConnector = serverConnector;
            closeService_.ServerConnector = serverConnector;
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            TestResultContainer resultInException = allTestResult_.FirstOrDefault(tr => tr.Exception != null);
            Exception ex = resultInException?.Exception ?? Exception;
            if (shouldThrowException && ex != null)
            {
                throw new EyesException("Error", ex);
            }
            return new TestResultsSummary(allTestResult_);
        }

        public void AggregateResult(TestResultContainer testResult)
        {
            allTestResult_.Add(testResult);
        }

        public RunningSession Open(string testId, SessionStartInfo sessionStartInfo)
        {
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(logger: Logger, testIds: testId);
            openService_.Operate(testId, sessionStartInfo, listener);
            RunningSession result = listener.Get();
            if (result == null)
            {
                throw new EyesException("Failed starting session with the server", listener.Exception);
            }
            return result;
        }

        public MatchResult Check(string testId, MatchWindowData matchWindowData)
        {
            SyncTaskListener listener = new SyncTaskListener(logger: Logger, testIds: testId);
            checkService_.TryUploadImage(testId, matchWindowData, listener);

            bool? result = listener.Get();
            if (result == null || result.Value == false)
            {
                throw new EyesException("Failed performing match with the server", listener.Exception);
            }

            SyncTaskListener<MatchResult> matchListener = new SyncTaskListener<MatchResult>(logger: Logger, testIds: testId);
            checkService_.MatchWindow(testId, matchWindowData, matchListener);
            return matchListener.Get();
        }

        public TestResults Close(string testId, SessionStopInfo sessionStopInfo)
        {
            SyncTaskListener<TestResults> listener = new SyncTaskListener<TestResults>(logger: Logger, testIds: testId);
            closeService_.Operate(testId, sessionStopInfo, listener);
            return listener.Get();
        }
    }
}
