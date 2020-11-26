using Applitools.Utils;
using CssParser;
using CssParser.Model.Rules;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

        public string GetFullWindowDom()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string domJson = GetDom_();
            logger_.Verbose(nameof(GetDom_) + " took {0} ms", stopwatch.Elapsed.TotalMilliseconds);
            return domJson;
        }

        internal string GetFullWindowDom(IPositionProvider positionProvider)
        {
            logger_.Verbose("enter");
            PositionMemento originalPosition = positionProvider.GetState();
            positionProvider.SetPosition(Point.Empty);
            Stopwatch stopwatch = Stopwatch.StartNew();
            string domJson = GetDom_();
            logger_.Verbose(nameof(GetDom_) + " took {0} ms", stopwatch.Elapsed.TotalMilliseconds);
            positionProvider.RestoreState(originalPosition);
            logger_.Verbose("exit");
            return domJson;
        }

        private string GetDom_()
        {
            FrameChain originalFC = null;
            originalFC = webDriver_.GetFrameChain().Clone();
            logger_.Verbose("saving current frame chain - size: {0} ; frame: {1}", originalFC.Count, originalFC.Peek());
            string dom = "";
            try
            {
                dom = GetFrameDom_();
            }
            catch (Exception e)
            {
                logger_.Log("Error: {0}", e);
            }

            logger_.Verbose("switching back to original frame");
            ((EyesWebDriverTargetLocator)webDriver_.SwitchTo()).Frames(originalFC);
            FrameChain currentFC = webDriver_.GetFrameChain();
            logger_.Verbose("switched to frame chain - size: {0} ; frame: {1}", currentFC.Count, currentFC.Peek());

            WaitForCssDownloadToFinish_();
            logger_.Verbose("finished waiting for CSS files to download");
            string inlaidString = StringUtils.EfficientStringReplace(cssStartToken_, cssEndToken_, dom, cssData_);
            logger_.Verbose("inlaid string");
            return inlaidString;
        }

        internal string GetDomCaptureAndPollingScriptResult_()
        {
            string captureScript = userAgent_.IsInternetExplorer ? CAPTURE_DOM_FOR_IE : CAPTURE_DOM;
            string pollingScript = userAgent_.IsInternetExplorer ? POLL_RESULT_FOR_IE : POLL_RESULT;

            int chunkByteLength = userAgent_.IsiOS ? 10 * MB : 256 * MB;

            object arguments = new { chunkByteLength };

            string result = null;
            try
            {
                result = EyesSeleniumUtils.RunDomScript(logger_, webDriver_, captureScript, arguments, arguments, pollingScript);
                result = JsonConvert.DeserializeObject<string>(result);
            }
            catch (JsonReaderException jsonException)
            {
                logger_.Log("Error: {0}", jsonException);
                logger_.Log("Error (cont.): Failed to parse string: " + result ?? "<null>");
            }
            catch (Exception e)
            {
                logger_.Log("Error capturing DOM");
                logger_.Log("Error: {0}", e);
            }
            return result;
        }

        private string GetFrameDom_()
        {
            logger_.Verbose("enter");

            string scriptResult = GetDomCaptureAndPollingScriptResult_();

            List<string> missingCssList = new List<string>();
            List<string> missingFramesList = new List<string>();
            List<string> data = new List<string>();
            Separators separators = ParseScriptResult(scriptResult, missingCssList, missingFramesList, data);
            cssStartToken_ = separators.CssStartToken;
            cssEndToken_ = separators.CssEndToken;

            FetchCssFiles_(missingCssList);

            Dictionary<string, string> framesData = RecurseFrames_(missingFramesList);
            string inlaidString = StringUtils.EfficientStringReplace(separators.IFrameStartToken, separators.IFrameEndToken, data[0], framesData);

            logger_.Verbose("exit");
            return inlaidString;
        }

        private void WaitForCssDownloadToFinish_()
        {
            AutoResetEvent[] whArr = null;
            lock (lockObject_)
            {
                whArr = new AutoResetEvent[waitHandles_.Count];
                waitHandles_.Values.CopyTo(whArr, 0);
            }
            logger_.Verbose("waiting on {0} wait handles:", whArr.Length);
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
                    logger_.Log("Not all wait handles recieved signal after {0} ms. aborting.", timeout);
                }
            }
        }

        private Separators ParseScriptResult(string scriptResult, List<string> missingCssList, List<string> missingFramesList, List<string> data)
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
                logger_.Verbose("missing css count: {0}", missingCssList.Count);
                logger_.Verbose("missing frames count: {0}", missingFramesList.Count);
                return separators;
            }
        }

        private void FetchCssFiles_(List<string> missingCssList)
        {
            logger_.Verbose("enter");
            List<CssTreeNode> cssTreeNodes = new List<CssTreeNode>();
            foreach (string missingCssUrl in missingCssList)
            {
                if (missingCssUrl.StartsWith("blob:") || missingCssUrl.StartsWith("data:"))
                {
                    logger_.Log("trying to download something impossible: {0}", missingCssUrl);
                    cssData_.Add(missingCssUrl, string.Empty);
                    continue;
                }
                logger_.Verbose("Downloading {0}", missingCssUrl);
                CssTreeNode cssTreeNode = new CssTreeNode(missingCssUrl, logger_, OnCssDownloadComplete_);
                cssTreeNodes.Add(cssTreeNode);
                AutoResetEvent waitHandle = new AutoResetEvent(false);
                logger_.Verbose("creating waithandle {0}", waitHandle.GetHashCode());
                waitHandles_[cssTreeNode] = waitHandle;
                cssTreeNode.Run();
            }
            logger_.Verbose("exit");
        }

        private Dictionary<string, string> RecurseFrames_(List<string> missingFramesList)
        {
            logger_.Verbose("enter");
            Dictionary<string, string> framesData = new Dictionary<string, string>();
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)webDriver_.SwitchTo();

            FrameChain fc = webDriver_.GetFrameChain().Clone();
            foreach (string missingFrameLine in missingFramesList)
            {
                try
                {
                    logger_.Verbose("handling frame xpath: {0}", missingFrameLine);

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
                        logger_.Log("WARNING! Failed switching into frame. HREF: {0}", locationAfterSwitch);
                        framesData.Add(missingFrameLine, string.Empty);
                        continue;
                    }
                    string result = GetFrameDom_();
                    framesData.Add(missingFrameLine, result);
                }
                catch (Exception e)
                {
                    logger_.Log("Error: {0}", e);
                    framesData.Add(missingFrameLine, string.Empty);
                }
                finally
                {
                    switchTo.Frames(fc);
                }
            }

            logger_.Verbose("exit");
            return framesData;
        }

        private void OnCssDownloadComplete_(object sender, EventArgs args)
        {
            logger_.Verbose("enter");
            CssTreeNode cssTreeNode = (CssTreeNode)sender;
            string css = cssTreeNode.CalcCss();
            string escapedCss = StringUtils.CleanForJSON(css);
            string href = cssTreeNode.Href;
            cssData_[href] = escapedCss;

            if (waitHandles_.TryGetValue(cssTreeNode, out AutoResetEvent waitHandle))
            {
                logger_.Verbose("calling 'set' on waithandle {0}", waitHandle.GetHashCode());
                waitHandle.Set();
                lock (lockObject_)
                {
                    waitHandles_.Remove(cssTreeNode);
                }
            }
            logger_.Verbose("exit");
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

            public void Run()
            {
                DownloadNodeCss_(this, this, Href);
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

            private void DownloadNodeCss_(CssTreeNode root, CssTreeNode targetCssTreeNode, string url)
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
                                    targetCssTreeNode.DownloadNodeCss_(root);
                                }
                                else
                                {
                                    logger_.Log("error downloading css {1}: {0}", args.Error, data.href);
                                    if (retriesCount_ > 0)
                                    {
                                        retriesCount_--;
                                        logger_.Log("Retrying...");
                                        Thread.Sleep(100);
                                        DownloadNodeCss_(root, this, Href);
                                    }
                                }
                                root.DecrementActiveDownloads();
                            }
                            catch (Exception e)
                            {
                                logger_.Log("Error: {0}", e);
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

            private void DownloadNodeCss_(CssTreeNode root)
            {
                if (allImportRules_.Count > 0)
                {
                    foreach (ImportRule importRule in allImportRules_)
                    {
                        CssTreeNode cssTreeNode = new CssTreeNode(logger_);
                        decendets_.Add(cssTreeNode);
                        string url = importRule.Href;
                        DownloadNodeCss_(root, cssTreeNode, url);
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
