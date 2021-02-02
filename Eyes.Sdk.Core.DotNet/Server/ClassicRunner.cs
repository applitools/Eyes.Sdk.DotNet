﻿using System;
using System.Collections.Generic;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        private readonly OpenService openService_;
        private readonly CheckService checkService_;
        private readonly CloseService closeService_;

        private List<TestResultContainer> allTestResult_ = new List<TestResultContainer>();

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
            return new TestResultsSummary(allTestResult_);
        }

        public void AggregateResult(TestResultContainer testResult)
        {
            allTestResult_.Add(testResult);
        }

        public RunningSession Open(SessionStartInfo sessionStartInfo)
        {
            SyncTaskListener<RunningSession> listener = new SyncTaskListener<RunningSession>(logger: Logger);
            openService_.Operate(sessionStartInfo, listener);
            return listener.Get();
        }

        public MatchResult Check(string testId, MatchWindowData matchWindowData)
        {
            SyncTaskListener listener = new SyncTaskListener(logger: Logger);
            checkService_.TryUploadImage(testId, matchWindowData, listener);

            bool? result = listener.Get();
            if (result == null || result.Value == false)
            {
                throw new EyesException("Failed performing match with the server", listener.Exception);
            }

            SyncTaskListener<MatchResult> matchListener = new SyncTaskListener<MatchResult>(logger: Logger);
            checkService_.MatchWindow(testId, matchWindowData, matchListener);
            return matchListener.Get();
        }

        public TestResults Close(string testId, SessionStopInfo sessionStopInfo)
        {
            SyncTaskListener<TestResults> listener = new SyncTaskListener<TestResults>(logger: Logger);
            closeService_.Operate(testId, sessionStopInfo, listener);
            return listener.Get();
        }
    }
}
