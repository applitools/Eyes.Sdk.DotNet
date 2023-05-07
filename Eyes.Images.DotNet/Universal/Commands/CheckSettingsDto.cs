/*using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Fluent;

namespace Applitools.Universal.Dto
{
    public class CheckSettingsDto
    {
        // screenshot
        private TRegion region_;
        private object frames_;
        private object webview_;
        private bool fully_;
        private TRegion scrollRootElement_;
        private string stitchMode_; // "CSS" | "Scroll"
        private bool hideScrollbars_; // this is not camel case
        private bool hideCaret_;
        private StitchOverlap overlap_;
        private int? waitBeforeCapture_;
        private LazyLoadOptions lazyLoad_;
        private bool ignoreDisplacements_;
        private NormalizationDto normalization_;
        private DebugScreenshotHandler debugImages_;

        // check
        private string name_;
        private string pageId_;
        private List<CodedRegionReference> ignoreRegions_;
        private List<CodedRegionReference> layoutRegions_;
        private List<CodedRegionReference> strictRegions_;
        private List<CodedRegionReference> contentRegions_;
        private object floatingRegions_;
        private object accessibilityRegions_;
        private AccessibilitySettings accessibilitySettings_;
        private string matchLevel_;
        private int? retryTimeout_;
        private bool sendDom_;
        private bool useDom_;
        private bool enablePatterns_;
        private bool ignoreCaret_;
        private Dictionary<string, object> ufgOptions_;
        private object layoutBreakpoints_;
        private bool disableBrowserFetching_;
        private AutProxyDto autProxy_;
        private Dictionary<string, string> hooks_;
        private List<IBrowsersInfo> renderers_;
        private string userCommandId_;
        private DensityMetrics densityMetrics_;

        public string CheckType { get; set; }

        public CheckSettingsDto()
        {
        }

        public TRegion GetRegion()
        {
            return region_;
        }

        public void SetRegion(TRegion region)
        {
            this.region_ = region;
        }

        public object GetFrames()
        {
            return frames_;
        }

        public void SetFrames(object frames)
        {
            this.frames_ = frames;
        }

        public bool GetFully()
        {
            return fully_;
        }

        public void SetFully(bool fully)
        {
            this.fully_ = fully;
        }

        public TRegion GetScrollRootElement()
        {
            return scrollRootElement_;
        }

        public void SetScrollRootElement(TRegion scrollRootElement)
        {
            this.scrollRootElement_ = scrollRootElement;
        }

        public string GetStitchMode()
        {
            return stitchMode_;
        }

        public void SetStitchMode(string stitchMode)
        {
            this.stitchMode_ = stitchMode;
        }

        public bool GetHideScrollbars()
        {
            return hideScrollbars_;
        }

        public void SetHideScrollbars(bool hideScrollbars)
        {
            this.hideScrollbars_ = hideScrollbars;
        }

        public bool GetHideCaret()
        {
            return hideCaret_;
        }

        public void SetHideCaret(bool hideCaret)
        {
            this.hideCaret_ = hideCaret;
        }

        public StitchOverlap GetOverlap()
        {
            return overlap_;
        }

        public void SetOverlap(StitchOverlap overlap)
        {
            this.overlap_ = overlap;
        }

        public int? GetWaitBeforeCapture()
        {
            return waitBeforeCapture_;
        }

        public void SetWaitBeforeCapture(int? waitBeforeCapture)
        {
            waitBeforeCapture_ = waitBeforeCapture;
        }

        public LazyLoadOptions GetLazyLoad()
        {
            return lazyLoad_;
        }

        public void SetLazyLoad(LazyLoadOptions lazyLoad)
        {
            this.lazyLoad_ = lazyLoad;
        }

        public bool GetIgnoreDisplacements()
        {
            return ignoreDisplacements_;
        }

        public void SetIgnoreDisplacements(bool ignoreDisplacements)
        {
            this.ignoreDisplacements_ = ignoreDisplacements;
        }

        public NormalizationDto GetNormalization()
        {
            return normalization_;
        }

        public void SetNormalization(NormalizationDto normalization)
        {
            this.normalization_ = normalization;
        }

        public DebugScreenshotHandlerDto GetDebugImages()
        {
            return debugImages_;
        }

        public void SetDebugImages(DebugScreenshotHandlerDto debugImages)
        {
            this.debugImages_ = debugImages;
        }

        public string GetName()
        {
            return name_;
        }

        public void SetName(string name)
        {
            this.name_ = name;
        }

        public string GetPageId()
        {
            return pageId_;
        }

        public void SetPageId(string pageId)
        {
            this.pageId_ = pageId;
        }

        public List<CodedRegionReference> GetIgnoreRegions()
        {
            return ignoreRegions_;
        }

        public void SetIgnoreRegions(List<CodedRegionReference> ignoreRegions)
        {
            this.ignoreRegions_ = ignoreRegions;
        }

        public List<CodedRegionReference> GetLayoutRegions()
        {
            return layoutRegions_;
        }

        public void SetLayoutRegions(List<CodedRegionReference> layoutRegions)
        {
            this.layoutRegions_ = layoutRegions;
        }

        public List<CodedRegionReference> GetStrictRegions()
        {
            return strictRegions_;
        }

        public void SetStrictRegions(List<CodedRegionReference> strictRegions)
        {
            this.strictRegions_ = strictRegions;
        }

        public List<CodedRegionReference> GetContentRegions()
        {
            return contentRegions_;
        }

        public void SetContentRegions(List<CodedRegionReference> contentRegions)
        {
            this.contentRegions_ = contentRegions;
        }

        public object GetFloatingRegions()
        {
            return floatingRegions_;
        }

        public void SetFloatingRegions(object floatingRegions)
        {
            this.floatingRegions_ = floatingRegions;
        }

        public object GetAccessibilityRegions()
        {
            return accessibilityRegions_;
        }

        public void SetAccessibilityRegions(object accessibilityRegions)
        {
            this.accessibilityRegions_ = accessibilityRegions;
        }

        public AccessibilitySettingsDto GetAccessibilitySettings()
        {
            return accessibilitySettings_;
        }

        public void SetAccessibilitySettings(AccessibilitySettingsDto accessibilitySettings)
        {
            this.accessibilitySettings_ = accessibilitySettings;
        }

        public string GetMatchLevel()
        {
            return matchLevel_;
        }

        public void SetMatchLevel(string matchLevel)
        {
            this.matchLevel_ = matchLevel;
        }

        public int? GetRetryTimeout()
        {
            return retryTimeout_;
        }

        public void SetRetryTimeout(int? retryTimeout)
        {
            this.retryTimeout_ = retryTimeout;
        }

        public bool GetSendDom()
        {
            return sendDom_;
        }

        public void SetSendDom(bool sendDom)
        {
            this.sendDom_ = sendDom;
        }

        public bool GetUseDom()
        {
            return useDom_;
        }

        public void SetUseDom(bool useDom)
        {
            this.useDom_ = useDom;
        }

        public bool GetEnablePatterns()
        {
            return enablePatterns_;
        }

        public void SetEnablePatterns(bool enablePatterns)
        {
            this.enablePatterns_ = enablePatterns;
        }

        public bool GetIgnoreCaret()
        {
            return ignoreCaret_;
        }

        public void SetIgnoreCaret(bool ignoreCaret)
        {
            this.ignoreCaret_ = ignoreCaret;
        }

        public Dictionary<string, object> GetUfgOptions()
        {
            return ufgOptions_;
        }

        public void SetUfgOptions(Dictionary<string, object> ufgOptions)
        {
            this.ufgOptions_ = ufgOptions;
        }

        public object GetLayoutBreakpoints()
        {
            return layoutBreakpoints_;
        }

        public void SetLayoutBreakpoints(object layoutBreakpoints)
        {
            this.layoutBreakpoints_ = layoutBreakpoints;
        }

        public bool GetDisableBrowserFetching()
        {
            return disableBrowserFetching_;
        }

        public void SetDisableBrowserFetching(bool disableBrowserFetching)
        {
            this.disableBrowserFetching_ = disableBrowserFetching;
        }

        public AutProxyDto GetAutProxy()
        {
            return autProxy_;
        }

        public void SetAutProxy(AutProxyDto autProxy)
        {
            this.autProxy_ = autProxy;
        }

        public Dictionary<string, string> GetHooks()
        {
            return hooks_;
        }

        public void SetHooks(Dictionary<string, string> hooks)
        {
            this.hooks_ = hooks;
        }

        public List<IBrowsersInfo> GetRenderers()
        {
            return renderers_;
        }

        public void SetRenderers(List<IBrowsersInfo> renderers)
        {
            this.renderers_ = renderers;
        }

        public void SetUserCommandId(string userCommandId)
        {
            this.userCommandId_ = userCommandId;
        }

        public string GetUserCommandId()
        {
            return userCommandId_;
        }

        public object GetWebview()
        {
            return webview_;
        }

        public void SetWebview(object webview)
        {
            this.webview_ = webview;
        }

        public DensityMetrics GetDensityMetrics()
        {
            return densityMetrics_;
        }

        public void SetDensityMetrics(DensityMetrics densityMetrics)
        {
            this.densityMetrics_ = densityMetrics;
        }

        public void SetCheckType(string checkType)
        {
            this.CheckType = checkType;
        }

        public string GetCheckType()
        {
            return CheckType;
        }
    }
}*/