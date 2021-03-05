using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

                Tuple<string, SessionStartInfo> nextInput = inputQueue_.Dequeue();
                string testId = nextInput.Item1;
                lock (lockObject_) inProgressTests_.Add(testId);
                Logger.Log(TraceLevel.Info, testId, Stage.Open, StageType.Run, new { testAmount = currentTestAmount_ });
                Operate(testId, nextInput.Item2, new TaskListener<RunningSession>(
                (output) =>
                {
                    lock (lockObject_)
                    {
                        inProgressTests_.Remove(testId);
                        outputQueue_.Add(Tuple.Create(testId, output));
                    }
                },
                (ex) =>
                {
                    Logger.Log(TraceLevel.Error, testId, Stage.Open, new { nextInput });
                    lock (lockObject_)
                    {
                        inProgressTests_.Remove(testId);
                        errorQueue_.Add(Tuple.Create(testId, ex));
                    }
                }));
            }
        }

        public void Operate(string testId, SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener)
        {
            Logger.Log(TraceLevel.Notice, testId, Stage.Open, new { sessionStartInfo.AgentSessionId });
            Stopwatch stopwatch = Stopwatch.StartNew();

            TaskListener<RunningSession> taskListener = new TaskListener<RunningSession>(
                (runningSession) => OnComplete_(sessionStartInfo, listener, runningSession, stopwatch, testId),
                (ex) => OnFail_(stopwatch, sessionStartInfo, listener, testId)
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

        private void OnComplete_(SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener,
            RunningSession runningSession, Stopwatch stopwatch, string testId)
        {
            if (runningSession.ConcurrencyFull)
            {
                isServerConcurrencyLimitReached_ = true;
                Logger.Log(TraceLevel.Warn, testId, Stage.Open, StageType.Retry,
                    new { message = "Failed starting test, concurrency is fully used. Trying again." });
                OnFail_(stopwatch, sessionStartInfo, listener, testId);
                return;
            }

            isServerConcurrencyLimitReached_ = false;
            Logger.Log(TraceLevel.Notice, testId, Stage.Open, StageType.Complete,
                    new { message = $"Server session ID is {runningSession.Id}" });
            //Logger.SessionId(runningSession.SessionId);
            listener.OnComplete(runningSession);
        }

        private void OnFail_(Stopwatch stopwatch, SessionStartInfo sessionStartInfo, TaskListener<RunningSession> listener, string testId)
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
                        (runningSession) => OnComplete_(sessionStartInfo, listener, runningSession, stopwatch, testId),
                        (ex) =>
                        {
                            if (ex.InnerException is System.Net.Http.HttpRequestException reqEx &&
                                reqEx.InnerException is System.Net.Sockets.SocketException socketEx &&
                                socketEx.SocketErrorCode == System.Net.Sockets.SocketError.ConnectionRefused)
                                throw ex;

                            OnFail_(stopwatch, sessionStartInfo, listener, testId);
                        }
                    ),
                    sessionStartInfo);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Open, StageType.Retry, e, testId);
                listener.OnFail(e);
            }
        }

        public int DecrementConcurrency()
        {
            return Interlocked.Decrement(ref currentTestAmount_);
        }
    }
}
