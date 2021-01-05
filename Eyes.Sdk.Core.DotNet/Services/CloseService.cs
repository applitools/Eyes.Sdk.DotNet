using System;

namespace Applitools
{
    public class CloseService : EyesService<SessionStopInfo, TestResults>
    {
        public CloseService(Logger logger, IServerConnector ufgConnector) : base(logger, ufgConnector)
        {
        }

        public override void Run()
        {
            while (inputQueue_.Count > 0)
            {
                Tuple<string, SessionStopInfo> nextInput = inputQueue_.Dequeue();
                string id = nextInput.Item1;
                Operate(nextInput.Item2, new TaskListener<TestResults>(
                    (output) =>
                    {
                        outputQueue_.Add(Tuple.Create(id, output));
                    },
                    (ex) =>
                    {
                        Logger.Log("Failed completing task on input {0}", nextInput);
                        lock (errorQueue_)
                        {
                            errorQueue_.Add(Tuple.Create(id, ex));
                        }
                    }
                    ));
            }
        }

        public void Operate(SessionStopInfo sessionStopInfo, TaskListener<TestResults> listener)
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
                Logger.Log("Session stopped successfully");
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
                ServerConnector.EndSession(taskListener, sessionStopInfo);
            }
            catch (Exception e)
            {
                listener.OnFail(e);
            }
        }
    }
}
