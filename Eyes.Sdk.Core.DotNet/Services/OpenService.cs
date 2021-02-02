using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Applitools
{
    public class OpenService : EyesService<SessionStartInfo, RunningSession>
    {
        private readonly TimeSpan TIME_TO_WAIT_FOR_OPEN = TimeSpan.FromMinutes(60);

        private int currentTestAmount_;
        private readonly int eyesConcurrency_;
        private bool isServerConcurrencyLimitReached_ = false;

        private readonly HashSet<string> inProgressTests_ = new HashSet<string>();

        public OpenService(Logger logger, IServerConnector serverConnector, int eyesConcurrency) : base(logger, serverConnector)
        {
            eyesConcurrency_ = eyesConcurrency;
        }

        public override void Run()
        {
            while (inputQueue_.Count > 0 && !isServerConcurrencyLimitReached_ && eyesConcurrency_ > currentTestAmount_)
            {
                Interlocked.Increment(ref currentTestAmount_);
                Logger.Log(TraceLevel.Info, testIds: null, Stage.Open, null, new { testAmount = currentTestAmount_ });

                Tuple<string, SessionStartInfo> nextInput = inputQueue_.Dequeue();
                string id = nextInput.Item1;
                inProgressTests_.Add(id);
                Operate(nextInput.Item2, new TaskListener<RunningSession>(
                (output) =>
                {
                    lock (outputQueue_)
                    {
                        inProgressTests_.Remove(id);
                        outputQueue_.Add(Tuple.Create(id, output));
                    }
                },
                (ex) =>
                {
                    Logger.Log("Failed completing task on input {0}", nextInput);
                    lock (errorQueue_)
                    {
                        inProgressTests_.Remove(id);
                        errorQueue_.Add(Tuple.Create(id, ex));
                    }
                }));
            }
        }

        public void Operate(SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener)
        {
            Logger.Log("Calling start session with agentSessionId {0}", sessionStartInfo.AgentSessionId);
            Stopwatch stopwatch = Stopwatch.StartNew();

            TaskListener<RunningSession> taskListener = new TaskListener<RunningSession>(
                (runningSession) => OnComplete_(sessionStartInfo, listener, runningSession, stopwatch),
                (ex) => OnFail_(stopwatch, sessionStartInfo, listener)
            );

            try
            {
                ServerConnector.StartSession(taskListener, sessionStartInfo);
            }
            catch (Exception e)
            {
                listener.OnFail(e);
            }
        }

        private void OnComplete_(SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener, RunningSession runningSession, Stopwatch stopwatch)
        {
            if (runningSession.ConcurrencyFull)
            {
                isServerConcurrencyLimitReached_ = true;
                Logger.Verbose("Failed starting test, concurrency is fully used. Trying again.");
                OnFail_(stopwatch, sessionStartInfo, listener);
                return;
            }

            isServerConcurrencyLimitReached_ = false;
            Logger.Verbose("Server session ID is {0}", runningSession.Id);
            //Logger.SessionId(runningSession.SessionId);
            listener.OnComplete(runningSession);
        }

        private void OnFail_(Stopwatch stopwatch, SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener)
        {
            TimeSpan sleepDuration = TimeSpan.FromSeconds(2);
            if (stopwatch.Elapsed > TIME_TO_WAIT_FOR_OPEN)
            {
                isServerConcurrencyLimitReached_ = false;
                listener.OnFail(new EyesException("Timeout in start session"));
                return;
            }

            try
            {
                Thread.Sleep(sleepDuration);
                if (stopwatch.Elapsed.TotalSeconds >= 30)
                {
                    sleepDuration = TimeSpan.FromSeconds(10);
                }
                else if (stopwatch.Elapsed.TotalSeconds >= 10)
                {
                    sleepDuration = TimeSpan.FromSeconds(5);
                }

                Logger.Verbose("Trying startSession again");
                ServerConnector.StartSession(
                    new TaskListener<RunningSession>(
                        (runningSession) => OnComplete_(sessionStartInfo, listener, runningSession, stopwatch),
                        (ex) => OnFail_(stopwatch, sessionStartInfo, listener)
                    ),
                    sessionStartInfo);
            }
            catch (Exception e)
            {
                listener.OnFail(e);
            }
        }

        public int DecrementConcurrency()
        {
            return Interlocked.Decrement(ref currentTestAmount_);
        }
    }
}
