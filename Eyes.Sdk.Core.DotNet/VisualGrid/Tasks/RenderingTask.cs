using Applitools.Fluent;
using Applitools.Utils;
using Applitools.VisualGrid.Model;
using CssParser;
using CssParser.Model;
using CssParser.Model.Rules;
using CssParser.Model.Values;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class RenderingTask : ICompletableTask
    {
        private const int FETCH_TIMEOUT_SECONDS = 60;
        private const int MAX_ITERATIONS = 30;

        private readonly RenderTaskListener listener_;
        private readonly List<RenderRequest> renderRequests_ = new List<RenderRequest>();
        private readonly List<VisualGridTask> checkTasks_ = new List<VisualGridTask>();
        private readonly IEyesConnector connector_;
        private readonly FrameData result_;
        private readonly IList<VisualGridSelector[]> regionSelectors_;
        private readonly ICheckSettings settings_;
        private readonly Logger logger_;

        // We use strings instead of Uris because Uri does not take the fragment (#) part into account when computing hash and when comparing Uris.
        private readonly ConcurrentDictionary<string, ResourceFuture> fetchedCacheMap_;

        internal static TimeSpan pollTimeout_ = TimeSpan.FromHours(1);
        private readonly bool isForcePutNeeded_;
#pragma warning disable CS0414
        private bool isTaskStarted_; // for debugging
        private bool isTaskInException; // for debugging
#pragma warning restore CS0414

        private RenderingInfo renderingInfo_;

        public bool IsTaskComplete { get; set; }

        public RenderingTask(IEyesConnector connector, RenderRequest renderRequest, VisualGridTask checkTask,
            VisualGridRunner runner, RenderTaskListener listener)
        {
            connector_ = connector;
            renderRequests_.Add(renderRequest);
            checkTasks_.Add(checkTask);
            fetchedCacheMap_ = runner.CachedResources;
            logger_ = runner.Logger;
            listener_ = listener;
        }

        public void Merge(RenderingTask renderingTask)
        {
            renderRequests_.AddRange(renderingTask.renderRequests_);
        }
       
        public class RenderTaskListener
        {
            public Action OnRenderSuccess;
            public Action<Exception> OnRenderFailed;

            public RenderTaskListener(Action onRenderSuccess, Action<Exception> onRenderFailed)
            {
                OnRenderSuccess = onRenderSuccess;
                OnRenderFailed = onRenderFailed;
            }
        }

        public async Task<RenderStatusResults> CallAsync()
        {
            try
            {
                logger_.Verbose("enter");
                logger_.Verbose("start rendering");

                List<RunningRender> runningRenders = null;
                RenderRequest[] requests = renderRequests_.ToArray();
                try
                {
                    runningRenders = await connector_.RenderAsync(requests);
                }
                catch (Exception e)
                {
                    logger_.Log("Error: " + e);
                    logger_.Verbose("/render throws exception. sleeping for 1.5s");
                    Thread.Sleep(1500);
                    try
                    {
                        runningRenders = await connector_.RenderAsync(requests);
                    }
                    catch (Exception e1)
                    {
                        SetRenderErrorToTasks_();
                        throw new EyesException("Invalid response for render request", e1);
                    }
                }
                logger_.Verbose("Validation render result");
                if (runningRenders == null || runningRenders.Count == 0)
                {
                    SetRenderErrorToTasks_();
                    throw new EyesException("Invalid response for render request");
                }

                for (int i = 0; i < renderRequests_.Count; i++)
                {
                    RenderRequest request = renderRequests_[i];
                    request.RenderId = runningRenders[i].RenderId;
                    logger_.Verbose("RunningRender: {0}", runningRenders[i]);
                }

                foreach (RunningRender runningRender in runningRenders)
                {
                    RenderStatus renderStatus = runningRender.RenderStatus;
                    if (renderStatus != RenderStatus.Rendered && renderStatus != RenderStatus.Rendering)
                    {
                        SetRenderErrorToTasks_();
                        throw new EyesException("Invalid response for render request. Status: " + renderStatus);
                    }
                }

                logger_.Verbose("Poll rendering status");
                Dictionary<RunningRender, RenderRequest> mapping = MapRequestToRunningRender_(runningRenders);
                PollRenderingStatus_(mapping);
            }
            catch (Exception e)
            {
                logger_.Log("Error: " + e);
                foreach (VisualGridTask checkTask in checkTasks_)
                {
                    checkTask.SetExceptionAndAbort(e);
                }
                listener_.OnRenderFailed(new EyesException("Failed rendering", e));
            }
            logger_.Verbose("Finished rendering task - exit");
            return null;
        }

        private void PollRenderingStatus_(Dictionary<RunningRender, RenderRequest> runningRenders)
        {
            logger_.Verbose("enter");
            List<string> ids = GetRenderIds_(runningRenders.Keys);
            Stopwatch stopwatch = Stopwatch.StartNew();
            do
            {
                List<RenderStatusResults> renderStatusResultsList = null;
                try
                {
                    renderStatusResultsList = connector_.RenderStatusById(ids.ToArray());
                }
                catch (Exception e)
                {
                    logger_.Log("Error (3): " + e);
                    continue;
                }
                if (renderStatusResultsList == null || renderStatusResultsList.Count == 0)
                {
                    logger_.Verbose("No reason to sample. (ids.Count: {0})", ids.Count);
                    Thread.Sleep(500);
                    continue;
                }
                if (renderStatusResultsList[0] == null)
                {
                    logger_.Verbose("First element is null. Total number of elements: {0}. Continuing.", renderStatusResultsList.Count);
                    Thread.Sleep(500);
                    continue;
                }
                SampleRenderingStatus_(runningRenders, ids, renderStatusResultsList);

                if (ids.Count > 0)
                {
                    Thread.Sleep(1500);
                }

                logger_.Verbose("ids.Count: {0} ; runtime: {1}", ids.Count, stopwatch.Elapsed);
            } while (ids.Count > 0 && stopwatch.Elapsed < pollTimeout_);

            foreach (string id in ids)
            {
                foreach (KeyValuePair<RunningRender, RenderRequest> kvp in runningRenders)
                {
                    RunningRender renderedRender = kvp.Key;
                    RenderRequest renderRequest = kvp.Value;
                    if (renderedRender.RenderId.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        VisualGridTask task = renderRequest.Task;
                        logger_.Verbose("removing failed render id: {0}", id);
                        task.SetRenderError(id, "too long rendering(rendering exceeded 150 sec)");
                        break;
                    }
                }
            }

            listener_.OnRenderSuccess();
            logger_.Verbose("exit");
        }

        private void SampleRenderingStatus_(Dictionary<RunningRender, RenderRequest> runningRenders, List<string> ids, List<RenderStatusResults> renderStatusResultsList)
        {
            logger_.Verbose("enter - renderStatusResultsList size: {0}", renderStatusResultsList.Count);

            for (int i = renderStatusResultsList.Count - 1; i >= 0; i--)
            {
                RenderStatusResults renderStatusResults = renderStatusResultsList[i];
                if (renderStatusResults == null)
                {
                    continue;
                }

                RenderStatus renderStatus = renderStatusResults.Status;
                bool isRenderedStatus = renderStatus == RenderStatus.Rendered;
                bool isErrorStatus = renderStatus == RenderStatus.Error;
                logger_.Verbose("renderStatusResults - {0}", renderStatusResults);
                if (isRenderedStatus || isErrorStatus)
                {
                    string removedId = ids[i];
                    ids.RemoveAt(i);

                    foreach (KeyValuePair<RunningRender, RenderRequest> kvp in runningRenders)
                    {
                        RunningRender renderedRender = kvp.Key;
                        RenderRequest renderRequest = kvp.Value;
                        string renderId = renderedRender.RenderId;
                        if (renderedRender.RenderId.Equals(removedId, StringComparison.OrdinalIgnoreCase))
                        {
                            VisualGridTask task = renderRequest.Task;

                            logger_.Verbose("setting task {0} render result: {1} to url {2}", task, renderStatusResults, result_.Url);
                            string error = renderStatusResults.Error;
                            if (error != null)
                            {
                                logger_.Log("Error: {0}", error);
                                task.SetRenderError(renderId, error);
                            }
                            task.SetRenderResult(renderStatusResults);
                            break;
                        }
                    }
                }
            }
            logger_.Verbose("exit");
        }

        public bool IsReady()
        {
            foreach (VisualGridTask checkTask in checkTasks_)
            {
                if (!checkTask.RunningTest.IsTestOpen) return false;
            }
            return true;
        }

        private List<string> GetRenderIds_(ICollection<RunningRender> runningRenders)
        {
            List<string> ids = new List<string>();
            foreach (RunningRender runningRender in runningRenders)
            {
                ids.Add(runningRender.RenderId);
            }
            return ids;
        }

        private void SetRenderErrorToTasks_()
        {
            foreach (RenderRequest renderRequest in renderRequests_)
            {
                renderRequest.Task.SetRenderError(null, "Invalid response for render request");
            }
        }

        private Dictionary<RunningRender, RenderRequest> MapRequestToRunningRender_(List<RunningRender> runningRenders)
        {
            Dictionary<RunningRender, RenderRequest> mapping = new Dictionary<RunningRender, RenderRequest>();
            for (int i = 0; i < renderRequests_.Count; i++)
            {
                RenderRequest request = renderRequests_[i];
                RunningRender runningRender = runningRenders[i];
                mapping.Add(runningRender, request);
            }
            return mapping;
        }
    }
}