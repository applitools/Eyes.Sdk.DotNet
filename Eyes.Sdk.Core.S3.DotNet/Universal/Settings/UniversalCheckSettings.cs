using System.Collections.Generic;
using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.Selenium.Commands
{
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

        public ExactMatchSettings Exact { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }

        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public bool? IgnoreDisplacements { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        public ICollection<TRegion> IgnoreRegions { get; set; }
        public ICollection<TRegion> LayoutRegions { get; set; }
        public ICollection<TRegion> StrictRegions { get; set; }
        public ICollection<TRegion> ContentRegions { get; set; }
        public ICollection<TFloatingRegion> FloatingRegions { get; set; }
        public ICollection<TFloatingRegion> AccessibilityRegions { get; set; }

        public TRegion Region { get; set; }
        public ICollection<IFrame> Frames { get; set; }
        public TRegion ScrollRootElement { get; set; }
        public bool? Fully { get; set; }

        public UniversalCheckSettings()
        {
            IgnoreRegions = new List<TRegion>();
            LayoutRegions = new List<TRegion>();
            StrictRegions = new List<TRegion>();
            ContentRegions = new List<TRegion>();
            FloatingRegions = new List<TFloatingRegion>();
            AccessibilityRegions = new List<TFloatingRegion>();
            Frames = new List<IFrame>();
            Timeout = -1;
        }
    }
}
