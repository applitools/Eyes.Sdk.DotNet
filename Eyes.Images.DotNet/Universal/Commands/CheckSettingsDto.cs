using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Fluent;

namespace Applitools.Universal.Dto
{
    public class CheckSettingsDto
    {
        // screenshot
        public TRegion Region { get; set; }
        public object Frames { get; set; }
        public object Webview { get; set; }
        public bool Fully { get; set; }
        public TRegion ScrollRootElement { get; set; }
        public string StitchMode { get; set; } // "CSS" | "Scroll"
        public bool HideScrollbars { get; set; } // this is not camel case
        public bool HideCaret { get; set; }
        public StitchOverlap Overlap { get; set; }
        public int? WaitBeforeCapture { get; set; }
        public LazyLoadOptions LazyLoad { get; set; }
        public bool IgnoreDisplacements { get; set; }
        public NormalizationDto Normalization { get; set; }
        public DebugScreenshotHandler DebugImages { get; set; }

        // check
        public string Name { get; set; }
        public string PageId { get; set; }
        public List<CodedRegionReference> IgnoreRegions { get; set; }
        public List<CodedRegionReference> LayoutRegions { get; set; }
        public List<CodedRegionReference> StrictRegions { get; set; }
        public List<CodedRegionReference> ContentRegions { get; set; }
        public object FloatingRegions { get; set; }
        public object AccessibilityRegions { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        public string MatchLevel { get; set; }
        public int? RetryTimeout { get; set; }
        public bool SendDom { get; set; }
        public bool UseDom { get; set; }
        public bool EnablePatterns { get; set; }
        public bool IgnoreCaret { get; set; }
        public Dictionary<string, object> UfgOptions { get; set; }
        public object LayoutBreakpoints { get; set; }
        public bool DisableBrowserFetching { get; set; }
        public ProxySettings AutProxy { get; set; }
        public Dictionary<string, string> Hooks { get; set; }
        public List<IRenderBrowserInfo> Renderers { get; set; }
        public string UserCommandId { get; set; }
        public DensityMetrics DensityMetrics { get; set; }

        public string CheckType { get; set; }
    }
}