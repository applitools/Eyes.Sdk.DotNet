using Applitools.Utils;
using CssParser;
using CssParser.Model.Rules;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace Applitools.Selenium.Capture
{
    internal abstract class DomCaptureBase
    {
        private static readonly int MB = 1024 * 1024;

        protected readonly Logger logger_;
        protected readonly CountdownEvent countdownEvent_ = new CountdownEvent(0);
        protected readonly Dictionary<string, string> cssData_ = new Dictionary<string, string>();
        protected string cssStartToken_;
        protected string cssEndToken_;
        private readonly IEyesJsExecutor jsExecutor_;
        protected readonly UserAgent userAgent_;

        public DomCaptureBase(Logger logger, IEyesJsExecutor jsExecutor, UserAgent userAgent)
        {
            logger_ = logger;
            jsExecutor_ = jsExecutor;
            userAgent_ = userAgent;
        }

        internal string GetFullWindowDom(params string[] testIds)
        {
            object originalFC = GetCurrentFrameChain();
            jsExecutor_.ExecuteScript("document.documentElement.setAttribute('data-applitools-active-frame', true)");
            SwitchToDefaultContent();
            Stopwatch stopwatch = Stopwatch.StartNew();
            string domJson = GetDom_(testIds);
            logger_.Verbose(nameof(GetDom_) + " took {0} ms", stopwatch.Elapsed.TotalMilliseconds);
            SwitchToFrameChain(originalFC);
            return domJson;
        }

        protected abstract void SwitchToFrame(string xpath);
        protected abstract void SwitchToDefaultContent();

        protected abstract object GetCurrentFrameChain();
        protected abstract void SwitchToFrameChain(object fc);
        private string GetDom_(string[] testIds)
        {
            object originalFC = GetCurrentFrameChain();
            string dom = "";
            countdownEvent_.Reset(1);
            try
            {
                dom = GetFrameDom(testIds);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.DomScript, e, testIds);
            }

            logger_.Verbose("switching back to original frame");
            SwitchToFrameChain(originalFC);
            countdownEvent_.Signal();
            WaitForCssDownloadToFinish(testIds);
            logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript,
                new { message = "finished waiting for CSS files to download" });
            string inlaidString = StringUtils.EfficientStringReplace(cssStartToken_, cssEndToken_, dom, cssData_);
            logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript, new { inlaidStringLength = inlaidString.Length });
            return inlaidString;
        }

        private string GetDomCaptureAndPollingScriptResult_(string[] testIds)
        {
            string captureScript = userAgent_.IsInternetExplorer ? Common.Resources.CAPTURE_DOM_FOR_IE : Common.Resources.CAPTURE_DOM;
            string pollingScript = userAgent_.IsInternetExplorer ? Common.Resources.POLL_CD_RESULT_FOR_IE : Common.Resources.POLL_CD_RESULT;

            int chunkByteLength = userAgent_.IsiOS ? 10 * MB : 256 * MB;

            object arguments = new { chunkByteLength };

            string result = null;
            try
            {
                result = Utils.RunDomScript(logger_, jsExecutor_, testIds, captureScript, arguments, arguments, pollingScript);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.DomScript, e, testIds);
            }
            return result;
        }

        protected string GetFrameDom(string[] testIds)
        {
            string scriptResult = GetDomCaptureAndPollingScriptResult_(testIds);

            List<string> missingCssList = new List<string>();
            List<string> missingFramesList = new List<string>();
            List<string> data = new List<string>();
            Separators separators = ParseScriptResult_(scriptResult, missingCssList, missingFramesList, data, testIds);
            cssStartToken_ = separators.CssStartToken;
            cssEndToken_ = separators.CssEndToken;

            FetchCssFiles_(missingCssList, testIds);

            Dictionary<string, string> framesData = RecurseFrames_(missingFramesList, testIds);
            string inlaidString = StringUtils.EfficientStringReplace(separators.IFrameStartToken, separators.IFrameEndToken, data[0], framesData);

            return inlaidString;
        }

        protected void WaitForCssDownloadToFinish(string[] testIds)
        {
            logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript,
                new { message = $"Waiting on {countdownEvent_.CurrentCount} to finish download" });

            const int timeout = 30000;
            bool allSignaled = countdownEvent_.Wait(timeout);
            if (!allSignaled)
            {
                logger_.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                    new { message = $"Not all wait handles recieved signal after {timeout} ms. aborting." });
            }
        }

        private Separators ParseScriptResult_(string scriptResult, List<string> missingCssList,
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
                logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript,
                        new { missingCssCount = missingCssList.Count, missingFramesCount = missingFramesList.Count });
                return separators;
            }
        }

        private void FetchCssFiles_(List<string> missingCssList, string[] testIds)
        {
            logger_.Log(TraceLevel.Info, testIds, Stage.Check, StageType.DomScript, new { missingCssList });
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
                countdownEvent_.AddCount();
                cssTreeNode.Run(testIds);
            }
        }

        private Dictionary<string, string> RecurseFrames_(List<string> missingFramesList, string[] testIds)
        {
            Dictionary<string, string> framesData = new Dictionary<string, string>();

            object fc = GetCurrentFrameChain();
            foreach (string missingFrameLine in missingFramesList)
            {
                try
                {
                    string originLocation = (string)jsExecutor_.ExecuteScript("return document.location.href");

                    string[] missingFrameXpaths = missingFrameLine.Split(',');
                    foreach (string missingFrameXpath in missingFrameXpaths)
                    {
                        SwitchToFrame(missingFrameXpath);
                    }
                    string locationAfterSwitch = (string)jsExecutor_.ExecuteScript("return document.location.href");
                    if (locationAfterSwitch.Equals(originLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        logger_.Log(TraceLevel.Warn, testIds, Stage.Check, StageType.DomScript,
                            new { message = "Failed switching into frame.", locationAfterSwitch });
                        framesData.Add(missingFrameLine, string.Empty);
                        continue;
                    }
                    string result = GetFrameDom(testIds);
                    framesData.Add(missingFrameLine, result);
                }
                catch (Exception e)
                {
                    CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, e, testIds);
                    framesData.Add(missingFrameLine, string.Empty);
                }
                finally
                {
                    SwitchToFrameChain(fc);
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
            countdownEvent_.Signal();
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
