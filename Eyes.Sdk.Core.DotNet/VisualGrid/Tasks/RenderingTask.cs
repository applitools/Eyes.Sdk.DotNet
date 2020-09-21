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
        private readonly IDebugResourceWriter debugResourceWriter_ = NullDebugResourceWriter.Instance;
        private readonly Logger logger_;

        // We use strings instead of Uris because Uri does not take the fragment (#) part into account when computing hash and when comparing Uris.
        private readonly ConcurrentDictionary<string, ResourceFuture> fetchedCacheMap_;
        private readonly ConcurrentDictionary<string, PutFuture> putResourceCache_;
        private readonly ConcurrentDictionary<string, byte> cachedBlobsUrls_;
        private readonly ConcurrentDictionary<string, IEnumerable<string>> cachedResourceMapping_;

        internal static TimeSpan pollTimeout_ = TimeSpan.FromHours(1);
        private readonly bool isForcePutNeeded_;
#pragma warning disable CS0414
        private bool isTaskStarted_; // for debugging
        private bool isTaskInException; // for debugging
#pragma warning restore CS0414

        private RenderingInfo renderingInfo_;

        public bool IsTaskComplete { get; set; }

        public RenderingTask(IEyesConnector connector, RenderRequest renderRequest, VisualGridTask checkTask,
            VisualGridRunner runner, RenderTaskListener listener, IDebugResourceWriter debugResourceWriter)
        {
            connector_ = connector;
            renderRequests_.Add(renderRequest);
            checkTasks_.Add(checkTask);
            fetchedCacheMap_ = runner.CachedResources;
            logger_ = runner.Logger;
            listener_ = listener;
            debugResourceWriter_ = debugResourceWriter;
        }

        public void Merge(RenderingTask renderingTask)
        {
            renderRequests_.AddRange(renderingTask.renderRequests_);
        }

        private void CollectBlobsFromFrameData_(FrameData frameData)
        {
            foreach (BlobData bd in frameData.Blobs)
            {
                string url = SanitizeUrl_(bd.Url.OriginalString);
                cachedBlobsUrls_.TryAdd(url, 0);
            }
            foreach (Uri uri in frameData.ResourceUrls)
            {
                string url = SanitizeUrl_(uri.OriginalString);
                cachedBlobsUrls_.TryAdd(url, 0);
            }
            foreach (FrameData fd in frameData.Frames)
            {
                string url = SanitizeUrl_(fd.Url.OriginalString);
                cachedBlobsUrls_.TryAdd(url, 0);
                CollectBlobsFromFrameData_(fd);
            }
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
                    runningRenders = connector_.Render(requests);
                }
                catch (Exception e)
                {
                    logger_.Log("Error: " + e);
                    logger_.Verbose("/render throws exception. sleeping for 1.5s");
                    Thread.Sleep(1500);
                    try
                    {
                        runningRenders = connector_.Render(requests);
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

        private RenderStatus CalcWorstStatus_(List<RunningRender> runningRenders, RenderStatus worstStatus)
        {
            foreach (RunningRender runningRender in runningRenders)
            {
                RenderStatus renderStatus = runningRender.RenderStatus;
                if (renderStatus == RenderStatus.NeedMoreResources && (worstStatus == RenderStatus.Rendered || worstStatus == RenderStatus.Rendering))
                {
                    worstStatus = RenderStatus.NeedMoreResources;
                }
                else if (renderStatus == RenderStatus.Error)
                {
                    worstStatus = RenderStatus.Error;
                    break;
                }
            }
            return worstStatus;
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

            ICheckSettingsInternal rcInternal = (ICheckSettingsInternal)settings_;
            logger_.Verbose("marking task as complete: {0}", rcInternal.GetName());
            IsTaskComplete = true;
            NotifySuccessAllListeners_();
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
            return checkTasks_[0].RunningTest.IsTestOpen;
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

        private void NotifySuccessAllListeners_()
        {
            foreach (RenderTaskListener listener in listener_)
            {
                listener.OnRenderSuccess();
            }
        }

        private void NotifyFailAllListeners_(Exception e)
        {
            foreach (RenderTaskListener listener in listener_)
            {
                listener.OnRenderFailed(e);
            }
        }

        private void SendMissingResources_(List<RunningRender> runningRenders, RGridDom dom, IDictionary<string, RGridResource> resources, bool isNeedMoreDom)
        {
            logger_.Verbose("enter - sending {0} missing resources. need more dom: {1}", resources.Count, isNeedMoreDom);
            List<PutFuture> allPuts = new List<PutFuture>();
            if (isNeedMoreDom)
            {
                RunningRender runningRender = runningRenders[0];
                PutFuture future = null;
                try
                {
                    future = connector_.RenderPutResource(runningRender, dom.AsResource());
                }
                catch (JsonException e)
                {
                    logger_.Log("Error (4): " + e);
                }
                logger_.Verbose("locking putResourceCache");
                lock (putResourceCache_)
                {
                    allPuts.Add(future);
                }
                logger_.Verbose("releasing putResourceCache");
            }

            logger_.Verbose("creating PutFutures for {0} runningRenders", runningRenders.Count);

            foreach (RunningRender runningRender in runningRenders)
            {
                CreatePutFutures_(allPuts, runningRender, resources);
            }

            logger_.Verbose("calling future.get on {0} PutFutures", allPuts.Count);
            foreach (PutFuture future in allPuts)
            {
                logger_.Verbose("calling future.get on {0}", future);
                try
                {
                    future.Get(TimeSpan.FromSeconds(120));
                }
                catch (Exception e)
                {
                    logger_.Log("Error (5): " + e);
                }
            }
            logger_.Verbose("exit");
        }

        internal void CreatePutFutures_(List<PutFuture> allPuts, RunningRender runningRender, IDictionary<string, RGridResource> resources)
        {
            List<Uri> needMoreResources = runningRender.NeedMoreResources;
            foreach (Uri url in needMoreResources)
            {
                string urlStr = SanitizeUrl_(url.OriginalString);
                if (putResourceCache_.TryGetValue(urlStr, out PutFuture putFuture))
                {
                    if (!allPuts.Contains(putFuture))
                    {
                        allPuts.Add(putFuture);
                    }
                    continue;
                }

                RGridResource resource;
                //logger_.Verbose("trying to get url from map - {0}", url);
                if (!fetchedCacheMap_.TryGetValue(urlStr, out ResourceFuture resourceFuture) || resourceFuture == null)
                {
                    logger_.Verbose("fetchedCacheMap.get(url) == null - " + url);
                    logger_.Log("Resource put requested but never downloaded (maybe a Frame)");
                    resources.TryGetValue(urlStr, out resource);
                }
                else
                {
                    try
                    {
                        Task<RGridResource> resourceTask = resourceFuture.Get(TimeSpan.FromSeconds(60));
                        resource = resourceTask.Result;
                    }
                    catch (Exception e)
                    {
                        logger_.Log("Error (6): " + e);
                        continue;
                    }
                }

                logger_.Verbose("resource({0}) hash : {1}", resource.Url, resource.Sha256);
                PutFuture future = connector_.RenderPutResource(runningRender, resource);
                if (resource.ContentType != RGridDom.ContentType)
                {
                    putResourceCache_.TryAdd(urlStr, future);
                }
                allPuts.Add(future);
            }
        }

        private Dictionary<RunningRender, RenderRequest> MapRequestToRunningRender_(List<RunningRender> runningRenders)
        {
            Dictionary<RunningRender, RenderRequest> mapping = new Dictionary<RunningRender, RenderRequest>();
            if (runningRenders != null && requests != null && runningRenders.Count >= requests.Length)
            {
                for (int i = 0; i < requests.Length; i++)
                {
                    RenderRequest request = requests[i];
                    RunningRender runningRender = runningRenders[i];
                    mapping.Add(runningRender, request);
                }
            }
            return mapping;
        }

        private void ForcePutAllResources_(IDictionary<string, RGridResource> resources, RunningRender runningRender)
        {
            /*
            RGridResource resource;
            List<PutFuture> allPuts = new List<PutFuture>();
            foreach (Uri url in resources.Keys)
            {
                try
                {
                    logger_.Verbose("trying to get url from map - {0}", url);
                    if (!fetchedCacheMap_.TryGetValue(url, out ResourceFuture resourceFuture))
                    {
                        logger_.Verbose("fetchedCacheMap.get(url) == null trying dom");
                        if (url.Equals(dom_.Url))
                        {
                            resource = dom_.AsResource();
                        }
                        else
                        {
                            logger_.Log("Resource not found Exiting...");
                            return;
                        }
                    }
                    else
                    {
                        resource = resourceFuture.Get();
                        PutFuture future = connector_.RenderPutResource(runningRender, resource);
                        logger_.Verbose("locking putResourceCache");
                        lock (putResourceCache_)
                        {
                            putResourceCache_.Add(dom_.Url, future);
                            allPuts.Add(future);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger_.Log("Error: " + e);
                }
            }
            foreach (PutFuture put in allPuts)
            {
                put.Get();
            }
            */
        }

        internal RenderRequest[] PrepareDataForRG_(FrameData result)
        {
            logger_.Verbose("enter");
            ConcurrentDictionary<string, RGridResource> allBlobs = new ConcurrentDictionary<string, RGridResource>();
            HashSet<string> resourceUrls = new HashSet<string>();

            ParseScriptResult_(result, allBlobs, resourceUrls);

            logger_.Verbose("fetching {0} resources...", resourceUrls.Count);

            //Fetch all resources
            FetchAllResources_(allBlobs, resourceUrls);

            logger_.Verbose("done fetching resources.");

            List<RGridResource> written = AddBlobsToCache_(allBlobs);

            logger_.Verbose("written {0} blobs to cache.", written.Count);

            //Create RenderingRequest

            //Parse allBlobs to mapping
            ConcurrentDictionary<string, RGridResource> resourceMapping = new ConcurrentDictionary<string, RGridResource>();
            foreach (KeyValuePair<string, RGridResource> kvp in allBlobs)
            {
                string urlStr = SanitizeUrl_(kvp.Key);
                ResourceFuture resourceFuture = fetchedCacheMap_[urlStr];
                Task<RGridResource> resourceTask = resourceFuture.Get(TimeSpan.FromSeconds(60));
                RGridResource resource = resourceTask.Result;
                resourceMapping.TryAdd(urlStr, resource); // TODO - ITAI
            }

            BuildAllRGDoms_(resourceMapping, result);

            logger_.Verbose("resources count: {0}", resourceMapping.Count);
            logger_.Verbose("cached resources count: {0}", cachedResourceMapping_.Count);

            AppendAllCachedResources_(resourceMapping);

            // Sort mapped resources by their URL for constant, testable results.
            SortedDictionary<string, RGridResource> sortedResourceMapping = new SortedDictionary<string, RGridResource>(resourceMapping);
            List<RenderRequest> allRequestsForRG = BuildRenderRequests_(result, sortedResourceMapping);

            RenderRequest[] asArray = allRequestsForRG.ToArray();

            logger_.Verbose("exit - returning renderRequest array of length: {0}", asArray.Length);
            return asArray;
        }

        private void AppendAllCachedResources_(ConcurrentDictionary<string, RGridResource> resourceMapping)
        {
            foreach (KeyValuePair<string, IEnumerable<string>> kvp in cachedResourceMapping_)
            {
                foreach (string url in kvp.Value)
                {
                    if (resourceMapping.ContainsKey(url)) continue;
                    if (fetchedCacheMap_.TryGetValue(url, out ResourceFuture value))
                    {
                        RGridResource res = value.GetResource();
                        if (res != null)
                        {
                            resourceMapping.TryAdd(url, res);
                        }
                    }
                }
            }
        }

        internal void ParseScriptResult_(FrameData currentFrame, ConcurrentDictionary<string, RGridResource> allBlobs, HashSet<string> resourceUrls)
        {
            logger_.Verbose("enter");
            Uri baseUrl = currentFrame.Url;
            logger_.Verbose("baseUrl: {0}", baseUrl);
            try
            {
                foreach (BlobData blob in currentFrame.Blobs)
                {
                    allBlobs.TryAdd(blob.Url.OriginalString, ParseBlobToGridResource_(baseUrl, blob));
                }

                foreach (Uri url in currentFrame.ResourceUrls)
                {
                    try
                    {
                        Uri uri = new Uri(baseUrl, url);
                        logger_.Verbose("adding resource to download: {0}", uri);
                        string sanitizedUrl = SanitizeUrl_(uri.OriginalString);
                        resourceUrls.Add(sanitizedUrl);
                    }
                    catch (UriFormatException e) { logger_.Log("Error (7): " + e); }
                }

                foreach (FrameData frame in currentFrame.Frames)
                {
                    ParseScriptResult_(frame, allBlobs, resourceUrls);
                }

                List<RGridResource> unparsedResources = AddBlobsToCache_(allBlobs);
                logger_.Verbose("written {0} blobs to cache.", unparsedResources.Count);
                ParseAndCollectExternalResources_(unparsedResources, baseUrl, resourceUrls);
                logger_.Verbose("collected {0} resources.", resourceUrls.Count);
            }
            catch (Exception e)
            {
                logger_.Log("Error (8): " + e);
            }
            logger_.Verbose("exit");
        }

        private void ParseAndCollectExternalResources_(IList<RGridResource> unparsedResources, Uri baseUrl, HashSet<string> resourceUrls)
        {
            logger_.Verbose("parsing {0} resources.", unparsedResources.Count);
            foreach (RGridResource blob in unparsedResources)
            {
                GetAndParseResource_(blob, baseUrl, resourceUrls);
            }
        }

        private void GetAndParseResource_(RGridResource blob, Uri baseUrl, HashSet<string> resourceUrls)
        {
            try
            {
                TextualDataResource tdr = TryGetTextualData_(blob, baseUrl, logger_);
                switch (tdr?.MimeType)
                {
                    case "text/css": ParseCSS_(tdr, resourceUrls, logger_); break;
                    case "image/svg+xml": ParseSVG_(tdr, resourceUrls, logger_); break;
                }
            }
            catch (Exception ex)
            {
                logger_.Log("Error: " + ex);
                logger_.Log("File name: " + blob.Url);
                debugResourceWriter_.Write(blob);
            }
        }

        internal class TextualDataResource
        {
            public string MimeType { get; set; }
            public Uri Uri { get; set; }
            public string Data { get; set; }
            public byte[] OriginalData { get; set; }
        }

        internal static TextualDataResource TryGetTextualData_(RGridResource blob, Uri baseUrl, Logger logger)
        {
            byte[] contentBytes = blob.Content;
            string contentTypeStr = blob.ContentType;
            logger.Verbose("enter - content length: {0} ; content type: {1}", contentBytes?.Length.ToString() ?? "<null>", contentTypeStr);
            if (contentTypeStr == null) return null;
            if (contentBytes.Length == 0) return null;
            string[] parts = contentTypeStr.Split(';');

            TextualDataResource tdr = new TextualDataResource();
            if (parts.Length > 0)
            {
                tdr.MimeType = parts[0].ToLowerInvariant();
            }

            string charset = "UTF-8";
            if (parts.Length > 1)
            {
                string[] keyVal = parts[1].Split('=');
                string key = keyVal[0].Trim();
                string val = keyVal[1].Trim().Trim('"');
                if (key.Equals("charset", StringComparison.OrdinalIgnoreCase))
                {
                    charset = val.ToUpper();
                }
            }

            if (charset != null)
            {
                tdr.Data = Encoding.GetEncoding(charset).GetString(contentBytes);
            }

            tdr.Uri = blob.Url;
            if (!tdr.Uri.IsAbsoluteUri)
            {
                tdr.Uri = new Uri(baseUrl, tdr.Uri);
            }

            tdr.OriginalData = blob.Content;
            logger.Verbose("exit");
            return tdr;
        }

        internal static void ParseCSS_(TextualDataResource tdr, HashSet<string> resourceUrls, Logger logger)
        {
            logger.Verbose("enter. Parsing CSS from {0}", tdr.Uri);
            Parser parser = new Parser();
            StyleSheet stylesheet = parser.Parse(tdr.Data);
            CollectAllImportUris_(stylesheet, resourceUrls, tdr.Uri, logger);
            CollectAllFontFaceUris_(stylesheet, resourceUrls, tdr.Uri, logger);
            CollectAllBackgroundImageUris_(stylesheet, resourceUrls, tdr.Uri, logger);
            logger.Verbose("exit");
        }

        internal static void ParseSVG_(TextualDataResource tdr, HashSet<string> allResourceUris, Logger logger)
        {
            HtmlDocument svgDoc = new HtmlDocument();
            svgDoc.Load(new MemoryStream(tdr.OriginalData));
            IEnumerable<HtmlNode> nodes = svgDoc.DocumentNode.Descendants().Where(n => n.HasAttributes);
            foreach (HtmlNode node in nodes)
            {
                foreach (HtmlAttribute attr in node.Attributes)
                {
                    if (attr.Name.Equals("xlink:href", StringComparison.OrdinalIgnoreCase) || attr.Name.Equals("href", StringComparison.OrdinalIgnoreCase))
                    {
                        string uri = attr.Value;
                        CreateUriAndAddToList(allResourceUris, tdr.Uri, uri, logger);
                    }
                }
            }
        }

        private static void CreateUriAndAddToList(HashSet<string> allResourceUris, Uri baseUrl, string uri, Logger logger)
        {
            if (uri.StartsWith("data:", StringComparison.OrdinalIgnoreCase) || uri.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
            {
                logger.Debug("Can't download `data:` and `javascript:` URLs");
                return;
            }

            try
            {
                Uri url = new Uri(baseUrl, uri);
                logger.Verbose("adding resource to download: {0}", url);
                string sanitizedUrl = SanitizeUrl_(url.OriginalString);
                allResourceUris.Add(sanitizedUrl);
            }
            catch (UriFormatException e)
            {
                logger.Log("Error (9): " + e);
            }
        }

        private static void CollectAllImportUris_(StyleSheet stylesheet, HashSet<string> allResourceUris, Uri baseUrl, Logger logger)
        {
            logger.Verbose("enter");
            IList<ImportRule> allImportRules = stylesheet.ImportDirectives;
            logger.Verbose("import rules count: {0}", allImportRules.Count);
            foreach (ImportRule importRule in allImportRules)
            {
                string uri = importRule.Href;
                CreateUriAndAddToList(allResourceUris, baseUrl, uri, logger);
            }
            logger.Verbose("exit");
        }

        private static void CollectAllFontFaceUris_(StyleSheet stylesheet, HashSet<string> allResourceUris, Uri baseUrl, Logger logger)
        {
            logger.Verbose("enter");
            IList<FontFaceRule> allFontFaceRules = stylesheet.FontFaceDirectives;
            logger.Verbose("font-face rules count: {0}", allFontFaceRules.Count);
            foreach (FontFaceRule fontFaceRule in allFontFaceRules)
            {
                foreach (Property property in fontFaceRule.Declarations)
                {
                    if (property.Name.Equals("src", StringComparison.OrdinalIgnoreCase))
                    {
                        Term term = property.Term;
                        CollectTermUris_(term, allResourceUris, baseUrl, logger);
                    }
                }
            }
            logger.Verbose("exit");
        }

        private static void CollectAllBackgroundImageUris_(StyleSheet stylesheet, HashSet<string> allResourceUris, Uri baseUrl, Logger logger)
        {
            logger.Verbose("enter");
            IList<StyleRule> allStyleRules = stylesheet.StyleRules;
            int count = 0;
            foreach (StyleRule styleRule in allStyleRules)
            {
                foreach (Property property in styleRule.Declarations)
                {
                    if (property.Name.Equals("background", StringComparison.OrdinalIgnoreCase) ||
                        property.Name.Equals("background-image", StringComparison.OrdinalIgnoreCase))
                    {
                        Term term = property.Term;
                        CollectTermUris_(term, allResourceUris, baseUrl, logger);
                        count++;
                    }
                }
            }
            logger.Verbose("background count: {0}", count);
            logger.Verbose("exit");
        }

        private static void CollectTermUris_(Term term, HashSet<string> allResourceUris, Uri baseUrl, Logger logger)
        {
            if (term is TermList termList)
            {
                foreach (Term innerTerm in termList)
                {
                    CollectTermUris_(innerTerm, allResourceUris, baseUrl, logger);
                }
            }
            else if (term is PrimitiveTerm primitiveTerm && primitiveTerm.PrimitiveType == UnitType.Uri)
            {
                string uri = primitiveTerm.Value.ToString();
                CreateUriAndAddToList(allResourceUris, baseUrl, uri, logger);
            }
        }

        private RGridResource ParseBlobToGridResource_(Uri baseUrl, BlobData blob)
        {
            byte[] content = null;
            if (blob.Value != null)
            {
                content = Convert.FromBase64String(blob.Value);
            }
            Uri url = blob.Url;
            if (!url.IsAbsoluteUri)
            {
                url = new Uri(baseUrl, url);
            }
            int? errorStatusCode = blob.ErrorStatusCode;
            RGridResource resource = new RGridResource(url, blob.Type, content, logger_, "parseBlobToGridResource", errorStatusCode);
            return resource;
        }

        private IDictionary<string, RGridResource> BuildAllRGDoms_(IDictionary<string, RGridResource> resourceMapping, FrameData currentFrame)
        {
            Uri baseUrl = currentFrame.Url;
            logger_.Verbose("enter - baseUrl: " + baseUrl);

            ConcurrentDictionary<string, RGridResource> mapping = new ConcurrentDictionary<string, RGridResource>();
            ConcurrentDictionary<string, RGridResource> frames = new ConcurrentDictionary<string, RGridResource>();

            logger_.Verbose("iterating {0} sub-frames", currentFrame.Frames.Count);
            foreach (FrameData frame in currentFrame.Frames)
            {
                logger_.Verbose("iterating {0} sub-frame blobs", frame.Blobs.Count);
                foreach (BlobData blob in frame.Blobs)
                {
                    string urlStr = blob.Url.OriginalString;
                    if (resourceMapping.TryGetValue(urlStr, out RGridResource rGridResource))
                    {
                        mapping.TryAdd(urlStr, rGridResource);
                    }
                }

                logger_.Verbose("iterating {0} sub-frame resource urls", frame.ResourceUrls.Count);
                foreach (Uri resourceUrl in frame.ResourceUrls)
                {
                    string urlStr = resourceUrl.OriginalString;
                    if (resourceMapping.TryGetValue(urlStr, out RGridResource rGridResource))
                    {
                        mapping.TryAdd(urlStr, rGridResource);
                    }
                }

                Uri frameUrl = frame.Url;
                /*if ("about:blank".Equals(frameUrl.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                else */
                if (!frameUrl.IsAbsoluteUri)
                {
                    frameUrl = new Uri(baseUrl, frameUrl);
                }


                try
                {
                    logger_.Verbose("entering recursion. url: {0}", frameUrl);
                    IDictionary<string, RGridResource> innerFrameResourceMap = BuildAllRGDoms_(resourceMapping, frame);
                    logger_.Verbose("exiting recursion. url: {0}", frameUrl);

                    foreach (KeyValuePair<string, RGridResource> kvp in innerFrameResourceMap)
                    {
                        mapping.TryAdd(kvp.Key, kvp.Value);
                    }
                    logger_.Verbose("mapped resources: {0}", mapping.Count);

                    // Sort mapped resources by their URL for constant, testable results.
                    SortedDictionary<string, RGridResource> sortedMapping = new SortedDictionary<string, RGridResource>(mapping);

                    RGridDom rGridDom = new RGridDom(frame.Cdt, sortedMapping, frameUrl, logger_, "buildAllRGDoms");
                    logger_.Verbose("adding resources to dictionary. url: {0}", frameUrl);
                    RGridResource frameResource = rGridDom.AsResource();
                    resourceMapping[frameUrl.OriginalString] = frameResource;
                    frames[frameUrl.OriginalString] = frameResource;
                }
                catch (Exception e)
                {
                    logger_.Log("Error (10): " + e);
                }
            }

            logger_.Verbose("exit");
            return frames;
        }

        private List<RGridResource> AddBlobsToCache_(ConcurrentDictionary<string, RGridResource> allBlobs)
        {
            logger_.Verbose("trying to add {0} blobs to cache", allBlobs.Count);
            logger_.Verbose("current fetchedCacheMap size: {0}", fetchedCacheMap_.Count);
            List<RGridResource> unparsedResources = new List<RGridResource>();
            foreach (RGridResource blob in allBlobs.Values)
            {
                Uri url = blob.Url;
                ResourceFuture resourceFuture = new ResourceFuture(blob, logger_);
                string sanitizedUrl = SanitizeUrl_(url.OriginalString);
                if (fetchedCacheMap_.TryAdd(sanitizedUrl, resourceFuture))
                {
                    logger_.Verbose("Cache write for url - {0} hash:({1})", url, resourceFuture.GetHashCode());
                    unparsedResources.Add(blob);
                }
                else
                {
                    logger_.Verbose("Already cached: url - {0} hash:({1})", url, resourceFuture.GetHashCode());
                }
            }
            return unparsedResources;
        }

        private Queue<string> resourcesToFetch_;
        internal void FetchAllResources_(IDictionary<string, RGridResource> allBlobs, HashSet<string> resourceUrls)
        {
            logger_.Verbose("enter");
            resourcesToFetch_ = new Queue<string>(resourceUrls);
            while (resourcesToFetch_.Count > 0)
            {
                var tasks = new List<Task<RGridResource>>();
                while (resourcesToFetch_.Count > 0)
                {
                    string url = resourcesToFetch_.Dequeue();
                    DownloadResource_(url, tasks);
                }

                while (tasks.Any(t => !t.IsCompleted))
                {
                    Task.WaitAll(tasks.ToArray());
                }

                foreach (Task<RGridResource> task in tasks)
                {
                    RGridResource resource = task.Result;
                    string url = resource.Url.OriginalString;
                    if (!allBlobs.ContainsKey(url))
                    {
                        allBlobs.Add(url, resource);
                    }
                }
            }
            logger_.Verbose("exit");
        }

        private void DownloadResource_(string url, List<Task<RGridResource>> tasks)
        {
            url = SanitizeUrl_(url);

            tasks.Add(Task.Run(async () =>
            {
                if (!fetchedCacheMap_.TryGetValue(url, out ResourceFuture future))
                {
                    future = DownloadResourceFuture_(url);
                    if (fetchedCacheMap_.TryAdd(url, future))
                    {
                        logger_.Verbose("fetchedCacheMap_.Add({0})", url);
                    }
                }
                RGridResource r = await future.Get(TimeSpan.FromSeconds(60));
                debugResourceWriter_.Write(r);
                HashSet<string> newResources = new HashSet<string>();
                GetAndParseResource_(r, r.Url, newResources);
                string sanitizedUrl = SanitizeUrl_(r.Url.OriginalString);
                cachedResourceMapping_.TryAdd(sanitizedUrl, newResources);
                logger_.Verbose("adding {0} more resources to download queue.", newResources.Count);
                foreach (string rUrl in newResources)
                {
                    sanitizedUrl = SanitizeUrl_(rUrl);
                    if (fetchedCacheMap_.ContainsKey(sanitizedUrl)) continue;
                    resourcesToFetch_.Enqueue(sanitizedUrl);
                    logger_.Verbose("added url to download: {0}", sanitizedUrl);
                }
                return r;
            }));
        }

        private ResourceFuture DownloadResourceFuture_(string url)
        {
            logger_.Verbose("Downloading data from {0}", url);
            // If resource is not being fetched yet (limited guarantee)
            IEyesConnector eyesConnector = checkTasks_?[0].EyesConnector;
            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.Proxy = eyesConnector?.Proxy;
            request.UserAgent = userAgent_.OriginalUserAgentString;
            Task<WebResponse> task = request.GetResponseAsync();
            ResourceFuture future = new ResourceFuture(task, new Uri(url), logger_);
            return future;
        }

        private static string SanitizeUrl_(string url)
        {
            int hashIndex = url.LastIndexOf('#');
            string sanitizedUrl = (hashIndex >= 0 ? url.Substring(0, hashIndex) : url).TrimEnd('?');
            return sanitizedUrl;
        }
    }
}