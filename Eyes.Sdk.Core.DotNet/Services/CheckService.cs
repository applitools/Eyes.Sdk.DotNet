using System;
using System.Collections.Generic;

namespace Applitools
{
    public class CheckService : EyesService<MatchWindowData, MatchResult>
    {
        private readonly Queue<Tuple<string, MatchWindowData>> matchWindowQueue_ = new Queue<Tuple<string, MatchWindowData>>();

        public CheckService(Logger logger, IServerConnector serverConnector) : base(logger, serverConnector)
        {
        }

        public override void Run()
        {
            while (inputQueue_.Count > 0)
            {
                Tuple<string, MatchWindowData> nextInput = inputQueue_.Dequeue();
                string id = nextInput.Item1;
                MatchWindowData matchWindowData = nextInput.Item2;
                TryUploadImage(matchWindowData,
                    new TaskListener(
                        () =>
                        {
                            matchWindowQueue_.Enqueue(Tuple.Create(id, matchWindowData));
                        },
                        (e) =>
                        {
                            Logger.Log("Failed completing task on input {0}", nextInput);
                            errorQueue_.Add(Tuple.Create(id, e));
                        }));
            }

            while (matchWindowQueue_.Count > 0)
            {
                Tuple<string, MatchWindowData> nextInput = matchWindowQueue_.Dequeue();
                string id = nextInput.Item1;
                MatchWindowData matchWindowData = nextInput.Item2;
                TaskListener<MatchResult> listener = new TaskListener<MatchResult>(
                (taskResponse) =>
                {
                    outputQueue_.Add(Tuple.Create(id, taskResponse));
                },
                (e) =>
                {
                    Logger.Log("Failed completing task on input {0}", nextInput);
                    errorQueue_.Add(new Tuple<string, Exception>(id, new EyesException("Match window failed")));
                });

                try
                {
                    ServerConnector.MatchWindow(listener, matchWindowData);
                }
                catch (Exception ex)
                {
                    Logger.Log("Error: {0}", ex);
                    listener.OnFail(ex);
                }
            };


        }

        public void TryUploadImage(MatchWindowData data, TaskListener taskListener)
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
                        Logger.Verbose("Got null url from upload");
                        appOutput.ScreenshotUrl = null;
                        taskListener.OnFail(new EyesException("Failed uploading image"));
                        return;
                    }

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
                ServerConnector.UploadImage(uploadListener, appOutput.ScreenshotBytes);
            }
            catch (Exception ex)
            {
                taskListener.OnFail(ex);
            }
        }


        public void MatchWindow(MatchWindowData data, TaskListener<MatchResult> listener)
        {
            try
            {
                ServerConnector.MatchWindow(listener, data);
            }
            catch (Exception ex)
            {
                listener.OnFail(ex);
            }
        }
    }
}