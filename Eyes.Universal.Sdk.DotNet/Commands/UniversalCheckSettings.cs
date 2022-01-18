using System.Collections.Generic;
using Applitools;
using Applitools.Utils.Geometry;

namespace Eyes.Universal.Sdk.DotNet.Commands
{
    public class RegionSelector
    {
        public string Type { get; set; }
        public string Selector { get; set; }
    }

    public class UniversalCheckSettings
    {
        public string Name { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public bool? LayoutBreakpoints { get; set; }
        public IDictionary<string, object> VisualGridOptions { get; set; }
        public string Hooks { get; set; }
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
           public MatchLevel? MatchLevel { get; set; }
           public bool? SendDom { get; set; }
           public bool? UseDom { get; set; }
           public bool? EnablePatterns { get; set; }
           public bool? IgnoreCaret { get; set; }
           public bool? IgnoreDisplacements { get; set; }
           public AccessibilitySettings AccessibilitySettings { get; set; }
           public IMutableRegion[] IgnoreRegions { get; set; }
           public IMutableRegion[] LayoutRegions { get; set; }
           public IMutableRegion[] StrictRegions { get; set; }
           public IMutableRegion[] ContentRegions { get; set; }
           public IMutableRegion[] FloatingRegions { get; set; }
           public IMutableRegion[] AccessibilityRegions { get; set; }

        /*
         export type ScreenshotSettings<TElement, TSelector> = {
          region?: RegionReference<TElement, TSelector>
          frames?: (ContextReference<TElement, TSelector> | FrameReference<TElement, TSelector>)[]
          scrollRootElement?: ElementReference<TElement, TSelector>
          fully?: boolean
        }
         */
        //[JsonProperty("region")]
        //public RegionSelector Selector { get; set; }
        //[JsonProperty("region")]
        //public MutableRegion Region { get; set; }
        public object Region { get; set; }
        public object Frames { get; set; }
        public object ScrollRootElement { get; set; }
        public bool? Fully { get; set; }
    }
}
