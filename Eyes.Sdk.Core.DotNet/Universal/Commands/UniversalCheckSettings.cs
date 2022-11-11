using System.Collections.Generic;
using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.Selenium.Commands
{
    public class Hooks
    {
        public string BeforeCaptureScreenshot { get; set; }
    }

    public class UniversalCheckSettings
    {
        public string Name { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public object LayoutBreakpoints { get; set; }
        public IDictionary<string, object> VisualGridOptions { get; set; }
        public Hooks Hooks { get; set; }
        public string RenderId { get; set; }
        public string VariationGroupId { get; set; }
        public int? Timeout { get; set; }

        /*
         export type MatchSettings<TRegion> = {
          exact?: {
            minDiffIntensity: number
            minDiffWidth: number
            minDiffHeight: number
            matchThreshold: number
          }
          matchLevel?: MatchLevel
          sendDom?: boolean
          useDom?: boolean
          enablePatterns?: boolean
          ignoreCaret?: boolean
          ignoreDisplacements?: boolean
          accessibilitySettings?: {
            level?: AccessibilityLevel
            guidelinesVersion?: AccessibilityGuidelinesVersion
          }
          ignoreRegions?: TRegion[]
          layoutRegions?: TRegion[]
          strictRegions?: TRegion[]
          contentRegions?: TRegion[]
          floatingRegions?: (TRegion | FloatingRegion<TRegion>)[]
          accessibilityRegions?: (TRegion | AccessibilityRegion<TRegion>)[]
        }
         */
        public ExactMatchSettings Exact { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }
        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public bool? IgnoreDisplacements { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        public CodedRegionReference[] IgnoreRegions { get; set; }
        public CodedRegionReference[] LayoutRegions { get; set; }
        public CodedRegionReference[] StrictRegions { get; set; }
        public CodedRegionReference[] ContentRegions { get; set; }
        public TFloatingRegion[] FloatingRegions { get; set; }
        public TAccessibilityRegion[] AccessibilityRegions { get; set; }

        /*
         export type ScreenshotSettings<TElement, TSelector> = {
          region?: RegionReference<TElement, TSelector>
          frames?: (ContextReference<TElement, TSelector> | FrameReference<TElement, TSelector>)[]
          scrollRootElement?: ElementReference<TElement, TSelector>
          fully?: boolean
        }
         */
        public TRegion Region { get; set; }
        public IList<IFrame> Frames { get; set; }
        public TRegion ScrollRootElement { get; set; }
        public bool? Fully { get; set; }

        public UniversalCheckSettings()
        {
            IgnoreRegions = new CodedRegionReference[0];
            LayoutRegions = new CodedRegionReference[0];
            StrictRegions = new CodedRegionReference[0];
            ContentRegions = new CodedRegionReference[0];
            FloatingRegions = new TFloatingRegion[0];
            AccessibilityRegions = new TAccessibilityRegion[0];
            Frames = new List<IFrame>();
            Timeout = -1;
        }
    }
}
