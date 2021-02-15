using System;
using System.Collections.Generic;

namespace Applitools
{
    public class CloseService : EyesService<SessionStopInfo, TestResults>
    {
        public CloseService(Logger logger, IServerConnector ufgConnector) : base(logger, ufgConnector)
        {
        }

        private readonly HashSet<string> inProgressTests_ = new HashSet<string>();

        public override void Run()
        {
            while (inputQueue_.Count > 0)
            {
                Tuple<string, SessionStopInfo> nextInput = inputQueue_.Dequeue();
                string id = nextInput.Item1;
                lock (lockObject_) inProgressTests_.Add(id);
                Operate(id, nextInput.Item2, new TaskListener<TestResults>(
                    (output) =>
                    {
                        lock (lockObject_)
                        {
                            inProgressTests_.Remove(id);
                            outputQueue_.Add(Tuple.Create(id, output));
                        }
                    },
                    (ex) =>
                    {
                        Logger.Log(TraceLevel.Error, id, Stage.Close, new { nextInput });
                        lock (lockObject_)
                        {
                            inProgressTests_.Remove(id);
                            errorQueue_.Add(Tuple.Create(id, ex));
                        }
                    }
                    ));
            }
        }

        public void Operate(string testId, SessionStopInfo sessionStopInfo, TaskListener<TestResults> listener)
        {
            if (sessionStopInfo == null)
            {
                TestResults testResults = new TestResults();
                testResults.Status = TestResultsStatus.NotOpened;
                listener.OnComplete(testResults);
                return;
            }

            TaskListener<TestResults> taskListener = new TaskListener<TestResults>(
            (testResults) =>
            {
                Logger.Log(TraceLevel.Notice, testId, Stage.Close, new { testResults.Status });
                testResults.IsNew = sessionStopInfo.RunningSession.IsNewSession;
                testResults.Url = sessionStopInfo.RunningSession.Url;
                Logger.Verbose(testResults.ToString());
                testResults.ServerConnector = ServerConnector as IDeleteSession;
                listener.OnComplete(testResults);
            },
            (ex) =>
            {
                listener.OnFail(new EyesException("Failed closing test"));
            });

            try
            {
                Logger.Log(TraceLevel.Notice, testId, Stage.Close, new { sessionStopInfo });
                ServerConnector.EndSession(taskListener, sessionStopInfo);
            }
            catch (Exception e)
            {
                listener.OnFail(e);
            }
        }
    }
}
