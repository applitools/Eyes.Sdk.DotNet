using Applitools.Utils;
using System;
using System.Collections.Generic;

namespace Applitools
{
    public class CheckService : EyesService<MatchWindowData, MatchResult>
    {
        private readonly Queue<Tuple<string, MatchWindowData>> matchWindowQueue_ = new Queue<Tuple<string, MatchWindowData>>();

        private readonly HashSet<string> inUploadProcess_ = new HashSet<string>();
        private readonly HashSet<string> inMatchWindowProcess_ = new HashSet<string>();

        public CheckService(Logger logger, IServerConnector serverConnector) : base(logger, serverConnector)
        {
        }

        public override void Run()
        {
            while (inputQueue_.Count > 0)
            {
                Tuple<string, MatchWindowData> nextInput = inputQueue_.Dequeue();
                string id = nextInput.Item1;
                lock (lockObject_) inUploadProcess_.Add(id);
                MatchWindowData matchWindowData = nextInput.Item2;
                TryUploadImage(id, matchWindowData,
                    new TaskListener(
                        () =>
                        {
                            lock (lockObject_)
                            {
                                inUploadProcess_.Remove(id);
                                matchWindowQueue_.Enqueue(Tuple.Create(id, matchWindowData));
                            }
                        },
                        (e) =>
                        {
                            lock (lockObject_)
                            {
                                inUploadProcess_.Remove(id);
                                Logger.Log(TraceLevel.Error, id, Stage.Check, StageType.UploadComplete, new { nextInput });
                                errorQueue_.Add(Tuple.Create(id, e));
                            }
                        }));
            }

            while (matchWindowQueue_.Count > 0)
            {
                Tuple<string, MatchWindowData> nextInput = matchWindowQueue_.Dequeue();
                string id = nextInput.Item1;
                lock (lockObject_) inMatchWindowProcess_.Add(id);
                MatchWindowData matchWindowData = nextInput.Item2;
                TaskListener<MatchResult> listener = new TaskListener<MatchResult>(
                (taskResponse) =>
                {
                    lock (lockObject_)
                    {
                        inMatchWindowProcess_.Remove(id);
                        outputQueue_.Add(Tuple.Create(id, taskResponse));
                    }
                },
                (e) =>
                {
                    lock (lockObject_)
                    {
                        inMatchWindowProcess_.Remove(id);
                        Logger.Log(TraceLevel.Error, id, Stage.Check, StageType.MatchComplete, new { nextInput });
                        errorQueue_.Add(new Tuple<string, Exception>(id, new EyesException("Match window failed")));
                    }
                });

                try
                {
                    MatchWindow(id, matchWindowData, listener);
                }
                catch (Exception ex)
                {
                    CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, StageType.MatchStart, ex, id);
                    listener.OnFail(ex);
                }
            };
        }

        public void TryUploadImage(string testId, MatchWindowData data, TaskListener taskListener)
        {
            AppOutput appOutput = data.AppOutput;
            if (appOutput.ScreenshotUrl != null)
            {
                taskListener.OnComplete();
                return;
            }

            // Getting the screenshot bytes
            TaskListener<string> uploadListener = new TaskListener<string>(
                (url) =>
                {
                    if (url == null)
                    {
                        Logger.Verbose("Got null url from upload. Test id: {0}", testId);
                        appOutput.ScreenshotUrl = null;
                        taskListener.OnFail(new EyesException("Failed uploading image"));
                        return;
                    }
                    Logger.Log(TraceLevel.Info, testId, Stage.Check, StageType.UploadComplete, new { url });
                    appOutput.ScreenshotUrl = url;
                    taskListener.OnComplete();
                },
                (ex) =>
                {
                    appOutput.ScreenshotUrl = null;
                    taskListener.OnFail(new EyesException("Failed uploading image", ex));
                }
            );

            try
            {
                Logger.Log(TraceLevel.Notice, testId, Stage.Check, StageType.UploadStart,
                    new { matchWindowQueue_ });
                ServerConnector.UploadImage(uploadListener, appOutput.ScreenshotBytes);
            }
            catch (Exception ex)
            {
                taskListener.OnFail(ex);
            }
        }

        public void MatchWindow(string testId, MatchWindowData data, TaskListener<MatchResult> listener)
        {
            try
            {
                Logger.Log(TraceLevel.Info, testId, Stage.Check, StageType.MatchStart, new { matchWindowData = data });
                LoggingListener<MatchResult> loggingListener = new LoggingListener<MatchResult>(listener, Logger, testId);
                ServerConnector.MatchWindow(loggingListener, data);
            }
            catch (Exception ex)
            {
                listener.OnFail(ex);
            }
        }
    }
}