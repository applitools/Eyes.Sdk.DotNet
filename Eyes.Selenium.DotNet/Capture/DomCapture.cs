using Applitools.Utils;
using CssParser;
using CssParser.Model.Rules;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Applitools.Selenium.Capture
{
    class DomCapture
    {
        private static readonly string CAPTURE_DOM = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_capture.dist.captureDomAndPoll.js");
        private static readonly string CAPTURE_DOM_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_capture.dist.captureDomAndPollForIE.js");
        private static readonly string POLL_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_capture.dist.pollResult.js");
        private static readonly string POLL_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.DotNet.Properties.NodeResources.node_modules._applitools.dom_capture.dist.pollResultForIE.js");

        private static readonly int MB = 1024 * 1024;

        internal static TimeSpan CAPTURE_TIMEOUT = TimeSpan.FromMinutes(5);

        private readonly Logger logger_;
        private readonly EyesWebDriver webDriver_;
        private readonly object lockObject_ = new object();
        private readonly Dictionary<CssTreeNode, AutoResetEvent> waitHandles_ = new Dictionary<CssTreeNode, AutoResetEvent>();
        private readonly Dictionary<string, string> cssData_ = new Dictionary<string, string>();
        private string cssStartToken_;
        private string cssEndToken_;
        private readonly UserAgent userAgent_;

        public DomCapture(Logger logger, EyesWebDriver webDriver, UserAgent userAgent)
        {
            logger_ = logger;
            webDriver_ = webDriver;
            userAgent_ = userAgent;
        }

        internal string GetFullWindowDom(params string[] testIds)
        {
            FrameChain originalFC = webDriver_.GetFrameChain().Clone();
            webDriver_.ExecuteScript("document.documentElement.setAttribute('data-applitools-active-frame', true)");
            webDriver_.SwitchTo().DefaultContent();
            Stopwatch stopwatch = Stopwatch.StartNew();
            string domJson = GetDom_(testIds);
            logger_.Verbose(nameof(GetDom_) + " took {0} ms", stopwatch.Elapsed.TotalMilliseconds);
            ((EyesWebDriverTargetLocator)webDriver_.SwitchTo()).Frames(originalFC);
            return domJson;
        }

        private string GetDom_(string[] testIds)
        {
            FrameChain originalFC = webDriver_.GetFrameChain().Clone();
            logger_.Verbose("saving current frame chain - size: {0} ; frame: {1}", originalFC.Count, originalFC.Peek());
            string dom = "";
            try
            {
                dom = GetFrameDom_(testIds);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.DomScript, e, testIds);
            }

            logger_.Verbose("switching back to original frame");
            ((EyesWebDriverTargetLocator)webDriver_.SwitchTo()).Frames(originalFC);
            FrameChain currentFC = webDriver_.GetFrameChain();
            logger_.Verbose("switched to frame chain - size: {0} ; frame: {1}", currentFC.Count, currentFC.Peek());

            WaitForCssDownloadToFinish_(testIds);
            logger_.Log("finished waiting for CSS files to download");
            string inlaidString = StringUtils.EfficientStringReplace(cssStartToken_, cssEndToken_, dom, cssData_);
            logger_.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.DomScript, new { inlaidStringLength = inlaidString.Length });
            return inlaidString;
        }

        internal string GetDomCaptureAndPollingScriptResult_(string[] testIds)
        {
            string captureScript = userAgent_.IsInternetExplorer ? CAPTURE_DOM_FOR_IE : CAPTURE_DOM;
            string pollingScript = userAgent_.IsInternetExplorer ? POLL_RESULT_FOR_IE : POLL_RESULT;

            int chunkByteLength = userAgent_.IsiOS ? 10 * MB : 256 * MB;

            object arguments = new { chunkByteLength };

            string result = null;
            try
            {
                result = EyesSeleniumUtils.RunDomScript(logger_, webDriver_, testIds, captureScript, arguments, arguments, pollingScript);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.DomScript, e, testIds);
            }
            return result;
        }

        private string GetFrameDom_(string[] testIds)
        {
            string scriptResult = GetDomCaptureAndPollingScriptResult_(testIds);

            List<string> missingCssList = new List<string>();
            List<string> missingFramesList = new List<string>();
            List<string> data = new List<string>();
            Separators separators = ParseScriptResult(scriptResult, missingCssList, missingFramesList, data, testIds);
            cssStartToken_ = separators.CssStartToken;
            cssEndToken_ = separators.CssEndToken;

            FetchCssFiles_(missingCssList, testIds);

            Dictionary<string, string> framesData = RecurseFrames_(missingFramesList, testIds);
            string inlaidString = StringUtils.EfficientStringReplace(separators.IFrameStartToken, separators.IFrameEndToken, data[0], framesData);

            return inlaidString;
        }

        private void WaitForCssDownloadToFinish_(string[] testIds)
        {
            AutoResetEvent[] whArr = null;
            lock (lockObject_)
            {
                whArr = new AutoResetEvent[waitHandles_.Count];
                waitHandles_.Values.CopyTo(whArr, 0);
            }
            logger_.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.DomScript,
                new { message = $"Waiting on {whArr.Length} to finish download" });

            foreach (AutoResetEvent wh in whArr)
            {
                logger_.Verbose("    " + wh.GetHashCode());
            }
            if (whArr.Length > 0)
            {
                const int timeout = 30000;
                bool allSignaled = WaitHandle.WaitAll(whArr, timeout);
                if (!allSignaled)
                {
                    logger_.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                        new { message = $"Not all wait handles recieved signal after {timeout} ms. aborting." });
                }
            }
        }

        private Separators ParseScriptResult(string scriptResult, List<string> missingCssList,
            List<string> missingFramesList, List<string> data, string[] testIds)
        {
            using (StringReader sr = new StringReader(scriptResult))
            {
                List<List<string>> blocks = new List<List<string>>() { missingCssList, missingFramesList, data };
                int index = 0;
                string json = sr.ReadLine();
                Separators separators = JsonConvert.DeserializeObject<Separators>(json);
                string str = sr.ReadLine();
                while (str != null)
                {
                    if (str == separators.Separator)
                    {
                        index++;
                    }
                    else
                    {
                        blocks[index].Add(str);
                    }
                    str = sr.ReadLine();
                }
                logger_.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.DomScript,
                        new { missingCssCount = missingCssList.Count, missingFramesCount = missingFramesList.Count });
                return separators;
            }
        }

        private void FetchCssFiles_(List<string> missingCssList, string[] testIds)
        {
            logger_.Log(TraceLevel.Notice, testIds, Stage.Check, StageType.DomScript, new { missingCssList });
            List<CssTreeNode> cssTreeNodes = new List<CssTreeNode>();
            foreach (string missingCssUrl in missingCssList)
            {
                if (missingCssUrl.StartsWith("blob:") || missingCssUrl.StartsWith("data:"))
                {
                    logger_.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                        new { message = "trying to download something impossible", missingCssUrl });
                    cssData_.Add(missingCssUrl, string.Empty);
                    continue;
                }
                CssTreeNode cssTreeNode = new CssTreeNode(missingCssUrl, logger_, OnCssDownloadComplete_);
                cssTreeNodes.Add(cssTreeNode);
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                waitHandles_[cssTreeNode] = waitHandle;
                cssTreeNode.Run(testIds);
            }
        }

        private Dictionary<string, string> RecurseFrames_(List<string> missingFramesList, string[] testIds)
        {
            Dictionary<string, string> framesData = new Dictionary<string, string>();
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)webDriver_.SwitchTo();

            FrameChain fc = webDriver_.GetFrameChain().Clone();
            foreach (string missingFrameLine in missingFramesList)
            {
                try
                {
                    string originLocation = (string)webDriver_.ExecuteScript("return document.location.href");

                    string[] missingFrameXpaths = missingFrameLine.Split(',');
                    foreach (string missingFrameXpath in missingFrameXpaths)
                    {
                        IWebElement frame = webDriver_.FindElement(By.XPath(missingFrameXpath));
                        switchTo.Frame(frame);
                    }
                    string locationAfterSwitch = (string)webDriver_.ExecuteScript("return document.location.href");
                    if (locationAfterSwitch.Equals(originLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        logger_.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                            new { message = "Failed switching into frame.", locationAfterSwitch });
                        framesData.Add(missingFrameLine, string.Empty);
                        continue;
                    }
                    string result = GetFrameDom_(testIds);
                    framesData.Add(missingFrameLine, result);
                }
                catch (Exception e)
                {
                    CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, e, testIds);
                    framesData.Add(missingFrameLine, string.Empty);
                }
                finally
                {
                    switchTo.Frames(fc);
                }
            }

            return framesData;
        }

        private void OnCssDownloadComplete_(object sender, EventArgs args)
        {
            CssTreeNode cssTreeNode = (CssTreeNode)sender;
            string css = cssTreeNode.CalcCss();
            string escapedCss = StringUtils.CleanForJSON(css);
            string href = cssTreeNode.Href;
            cssData_[href] = escapedCss;

            if (waitHandles_.TryGetValue(cssTreeNode, out AutoResetEvent waitHandle))
            {
                waitHandle.Set();
                lock (lockObject_)
                {
                    waitHandles_.Remove(cssTreeNode);
                }
            }
        }

        private class CssTreeNode
        {
            class WebClientUserData
            {
                public Uri href { get; set; }
                public Stopwatch stopwatch { get; set; }
            }

            private readonly IList<CssTreeNode> decendets_ = new List<CssTreeNode>();
            private string css_;
            private IList<ImportRule> allImportRules_;
            private event EventHandler downloadComplete_;
            private int activeDownloads_ = 0;
            private Logger logger_;
            private int retriesCount_ = 1;

            public string Href { get; private set; }

            private CssTreeNode(Logger logger)
            {
                logger_ = logger;
            }

            public CssTreeNode(string href, Logger logger, EventHandler onDownloadComplete)
            {
                ArgumentGuard.NotNull(onDownloadComplete, nameof(onDownloadComplete));
                logger_ = logger;
                downloadComplete_ = onDownloadComplete;
                Href = href;
            }

            public void Run(params string[] testIds)
            {
                DownloadNodeCss_(this, this, Href, testIds);
            }

            public string CalcCss()
            {
                int size = CalcCssBufferSize_();
                StringBuilder sb = new StringBuilder(size);
                CalcCss_(sb);
                return sb.ToString();
            }

            private int CalcCssBufferSize_()
            {
                if (css_ == null) return 0;
                int size = css_.Length;
                foreach (CssTreeNode decendet in decendets_)
                {
                    size += decendet.CalcCssBufferSize_();
                }
                return size;
            }

            private void CalcCss_(StringBuilder sb)
            {
                foreach (CssTreeNode decendet in decendets_)
                {
                    decendet.CalcCss_(sb);
                }
                sb.Append(css_);
            }

            private void DownloadCssAsync_(string url, Logger logger, DownloadStringCompletedEventHandler onDownloadCompleted)
            {
                logger?.Verbose("Given URL to download: {0}", url);
                Uri href = new Uri(new Uri(Href), url);
                WebClient webClient = new WebClient();
                logger?.Verbose("Downloading CSS from {0}", href);
                webClient.DownloadStringCompleted += onDownloadCompleted;
                webClient.DownloadStringAsync(href, new WebClientUserData { href = href, stopwatch = Stopwatch.StartNew() });
            }

            private void DownloadNodeCss_(CssTreeNode root, CssTreeNode targetCssTreeNode, string url, string[] testIds)
            {
                root.IncrementActiveDownloads();
                DownloadCssAsync_(url, root.logger_,
                        (object sender, DownloadStringCompletedEventArgs args) =>
                        {
                            try
                            {
                                WebClientUserData data = (WebClientUserData)args.UserState;
                                if (args.Error == null)
                                {
                                    string css = args.Result;
                                    WebClient webClient = (WebClient)sender;
                                    logger_.Verbose("download completed for {0}", data.href);
                                    logger_.Verbose("download took {0} ms", data.stopwatch.Elapsed.TotalMilliseconds);
                                    webClient.Dispose();
                                    ParseCss_(targetCssTreeNode, css);
                                    targetCssTreeNode.DownloadNodeCss_(root, testIds);
                                }
                                else
                                {
                                    logger_.Log(TraceLevel.Error, testIds, Stage.Check, StageType.DownloadResource,
                                        new { args.Error, data.href });
                                    if (retriesCount_ > 0)
                                    {
                                        retriesCount_--;
                                        logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.Retry);
                                        Thread.Sleep(100);
                                        DownloadNodeCss_(root, this, Href, testIds);
                                    }
                                }
                                root.DecrementActiveDownloads();
                            }
                            catch (Exception e)
                            {
                                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, e, testIds);
                            }
                        });
            }

            private void IncrementActiveDownloads()
            {
                Interlocked.Increment(ref activeDownloads_);
                logger_.Verbose("new active downloads: {0}", activeDownloads_);
            }

            private void DecrementActiveDownloads()
            {
                Interlocked.Decrement(ref activeDownloads_);
                logger_.Verbose("new active downloads: {0}", activeDownloads_);
                if (activeDownloads_ == 0)
                {
                    logger_.Verbose("no more active downloads for this node. calling download complete callback. ({0})", this.GetHashCode());
                    downloadComplete_(this, EventArgs.Empty);
                }
            }

            private void DownloadNodeCss_(CssTreeNode root, string[] testIds)
            {
                if (allImportRules_.Count > 0)
                {
                    foreach (ImportRule importRule in allImportRules_)
                    {
                        CssTreeNode cssTreeNode = new CssTreeNode(logger_);
                        decendets_.Add(cssTreeNode);
                        string url = importRule.Href;
                        DownloadNodeCss_(root, cssTreeNode, url, testIds);
                    }
                }
            }

            private static void ParseCss_(CssTreeNode targetCssTreeNode, string css)
            {
                Parser parser = new Parser();
                StyleSheet stylesheet = parser.Parse(css);
                StringBuilder sb = new StringBuilder(2048);
                foreach (RuleSet rule in stylesheet.Rules)
                {
                    if (rule.RuleType != CssParser.Model.RuleType.Import)
                    {
                        sb.Append(rule.ToString());
                    }
                }
                targetCssTreeNode.allImportRules_ = stylesheet.ImportDirectives;
                targetCssTreeNode.css_ = sb.ToString();
            }
        }
    }
}
