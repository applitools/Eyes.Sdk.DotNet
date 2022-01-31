using System.Collections.Generic;
using Applitools;
using Applitools.Fluent;

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
        public bool? LayoutBreakpoints { get; set; }
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
           public MatchLevel? MatchLevel { get; set; }
           public bool? SendDom { get; set; }
           public bool? UseDom { get; set; }
           public bool? EnablePatterns { get; set; }
           public bool? IgnoreCaret { get; set; }
           public bool? IgnoreDisplacements { get; set; }
           public AccessibilitySettings AccessibilitySettings { get; set; }
           public TRegion[] IgnoreRegions { get; set; }
           public TRegion[] LayoutRegions { get; set; }
           public TRegion[] StrictRegions { get; set; }
           public TRegion[] ContentRegions { get; set; }
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
            IgnoreRegions = new TRegion[0];
            LayoutRegions = new TRegion[0];
            StrictRegions = new TRegion[0];
            ContentRegions = new TRegion[0];
            FloatingRegions = new TFloatingRegion[0];
            AccessibilityRegions = new TAccessibilityRegion[0];
            Frames = new List<IFrame>();
            Timeout = -1;
        }
    }
}
