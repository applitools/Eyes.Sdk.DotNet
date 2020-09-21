using Applitools.Fluent;
using Applitools.Utils;
using Applitools.VisualGrid.Model;
using CssParser;
using CssParser.Model;
using CssParser.Model.Rules;
using CssParser.Model.Values;
using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class ResourceCollectionTask
    {
        private readonly Logger logger_;
        private readonly VisualGridRunner runner_;
        private readonly IEyesConnector eyesConnector_;
        private readonly FrameData domData_;
        private readonly UserAgent userAgent_;
        private readonly RenderingInfo renderingInfo_;
        private readonly List<VisualGridSelector[]> regionSelectors_;
        private readonly ICheckSettings checkSettings_;
        private readonly List<VisualGridTask> checkTasks_;

        // We use strings instead of Uris because Uri does not take the fragment (#) part into account when computing hash and when comparing Uris.
        private readonly ConcurrentDictionary<string, ResourceFuture> fetchedCacheMap_;
        private readonly ConcurrentDictionary<string, PutFuture> putResourceCache_;
        private readonly ConcurrentDictionary<string, byte> cachedBlobsUrls_;
        private readonly ConcurrentDictionary<string, IEnumerable<string>> cachedResourceMapping_;

        private readonly HashSet<string> uploadedResourcesCache_;
        private IDebugResourceWriter debugResourceWriter_;
        private readonly TaskListener<List<RenderingTask>> listener_;
        private RenderingTask.RenderTaskListener renderTaskListener_;

        public TestResultContainer Call()
        {
            try
            {
                DomAnalyzer domAnalyzer = new DomAnalyzer(logger, eyesConnector.getServerConnector(),
                        debugResourceWriter, domData, fetchedCacheMap, userAgent);
                Dictionary<string, RGridResource> resourceMap = domAnalyzer.analyze();
                List<RenderRequest> renderRequests = BuildRenderRequests_(domData_, resourceMap);
                if (debugResourceWriter_ != null && !(debugResourceWriter_ is NullDebugResourceWriter))
                {
                    foreach (RenderRequest renderRequest in renderRequests)
                    {
                        try
                        {
                            debugResourceWriter_.Write(renderRequest.Dom.AsResource());
                        }
                        catch (Exception e)
                        {
                            logger_.Log("Error: " + e);
                        }
                        foreach (RGridResource value in renderRequest.Resources.Values)
                        {
                            debugResourceWriter_.Write(value);
                        }
                    }
                }

                logger_.Verbose("exit - returning renderRequest array of length: {0}", renderRequests.Count);

                List<RenderingTask> renderingTasks = new List<RenderingTask>();
                for (int i = 0; i < renderRequests.Count; i++)
                {
                    VisualGridTask checkTask = checkTasks_[i];
                    checkTask.IsTaskReadyForRender = true;
                    renderingTasks.Add(new RenderingTask(eyesConnector_, renderRequests[i], checkTask, runner_, renderTaskListener_));
                }

                logger_.Verbose("Uploading missing resources");
                ConcurrentDictionary<string, RGridResource> missingResources = CheckResourcesStatus_(renderRequests[0].Dom, resourceMap);
                UploadResources_(missingResources);
                try
                {
                    if (phaser.getRegisteredParties() > 0)
                    {
                        phaser.awaitAdvanceInterruptibly(0, 30, TimeUnit.SECONDS);
                    }
                }
                catch (Exception e)
                {
                    GeneralUtils.logExceptionStackTrace(logger, e);
                    phaser.forceTermination();
                }

                listener.onComplete(renderingTasks);
            }
            catch (Throwable t)
            {
                GeneralUtils.logExceptionStackTrace(logger, t);
                listener.onFail();
            }

            return null;
        }


        /// <summary>
        /// Checks with the server what resources are missing.
        /// </summary>
        /// <param name="dom"></param>
        /// <param name="resourceMap"></param>
        /// <returns>All the missing resources to upload.</returns>
        private Dictionary<string, ResourceFuture> CheckResourcesStatus_(RGridDom dom, Dictionary<string, RGridResource> resourceMap)
        {
            List<HashObject> hashesToCheck = new List<HashObject>();
            List<string> urlsToCheck = new List<string>();
            foreach (KeyValuePair<string, RGridResource> kvp in resourceMap)
            {
                string url = kvp.Key;
                RGridResource resource = kvp.Value;
                string hash = resource.Sha256;
                string hashFormat = resource.HashFormat;
                lock (uploadedResourcesCache_)
                {
                    if (!uploadedResourcesCache_.Contains(hash))
                    {
                        hashesToCheck.Add(new HashObject(hashFormat, hash));
                        urlsToCheck.Add(url);
                    }
                }
            }

            RGridResource domResource = dom.AsResource();
            lock (uploadedResourcesCache_)
            {
                if (!uploadedResourcesCache_.Contains(domResource.Sha256))
                {
                    hashesToCheck.Add(new HashObject(domResource.HashFormat, domResource.Sha256));
                    string uri = SanitizeUrl_(domResource.Url.OriginalString);
                    urlsToCheck.Add(uri);
                }
            }

            if (hashesToCheck.Count == 0)
            {
                return new Dictionary<string, ResourceFuture>();
            }

            object lockObject = new object();// AtomicReference<>(new EyesSyncObject(logger, "checkResourceStatus"));
            //AtomicReference<Boolean[]> reference = new AtomicReference<>();
            //SyncTaskListener<Boolean[]> listener = new SyncTaskListener<>(lock, reference);
            eyesConnector_.CheckResourceStatus_(listener_, null, hashesToCheck.ToArray());
            lock (lockObject)
            {
                try
                {
                    lockObject.waitForNotify();
                }
                catch (ThreadInterruptedException) { }
            }

            bool[] result = reference.get();
            if (result == null)
            {
                return new Dictionary<string, ResourceFuture>();
            }

            Dictionary<string, ResourceFuture> missingResources = new Dictionary<string, ResourceFuture>();
            for (int i = 0; i < result.Length; i++)
            {
                if (result[i] != null && result[i])
                {
                    continue;
                }

                string resourceUrl = urlsToCheck[i];
                missingResources.Add(resourceUrl, fetchedCacheMap_[resourceUrl]);
            }

            return missingResources;
        }

        private List<RenderRequest> BuildRenderRequests_(FrameData currentFrame, IDictionary<string, RGridResource> resourceMapping)
        {
            Uri url = currentFrame.Url;
            RGridDom dom = new RGridDom(currentFrame.Cdt, resourceMapping, url, logger_, "buildRenderRequests");
            ResourceFuture domResourceFuture = new ResourceFuture(dom.AsResource(), logger_);
            string uri = SanitizeUrl_(url.OriginalString);
            fetchedCacheMap_.TryAdd(uri, domResourceFuture);

            //Create RG requests
            List<RenderRequest> allRequestsForRG = new List<RenderRequest>();
            ICheckSettingsInternal csInternal = (ICheckSettingsInternal)checkSettings_;

            foreach (VisualGridTask task in checkTasks_)
            {
                RenderBrowserInfo browserInfo = task.BrowserInfo;
                RenderInfo renderInfo = new RenderInfo(browserInfo.Width, browserInfo.Height,
                    csInternal.GetSizeMode(), csInternal.GetTargetSelector(),
                    csInternal.GetTargetRegion(), browserInfo.EmulationInfo, browserInfo.IosDeviceInfo);

                List<VisualGridSelector> regionSelectors = new List<VisualGridSelector>();
                if (regionSelectors_ != null)
                {
                    foreach (VisualGridSelector[] vgs in regionSelectors_)
                    {
                        regionSelectors.AddRange(vgs);
                    }
                }

                RenderRequest request = new RenderRequest(renderingInfo_.ResultsUrl, url,
                        renderingInfo_.StitchingServiceUrl, dom,
                        resourceMapping, renderInfo, browserInfo.Platform,
                        browserInfo.BrowserType, csInternal.GetScriptHooks(),
                        regionSelectors.ToArray(), csInternal.GetSendDom() ?? false, task,
                        csInternal.GetVisualGridOptions());

                allRequestsForRG.Add(request);
            }
            return allRequestsForRG;
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

        private static string SanitizeUrl_(string url)
        {
            int hashIndex = url.LastIndexOf('#');
            string sanitizedUrl = (hashIndex >= 0 ? url.Substring(0, hashIndex) : url).TrimEnd('?');
            return sanitizedUrl;
        }
    }
}
