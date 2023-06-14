using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public class UniversalCheckSettings : SettingsBase
    {
        public bool? IgnoreDisplacements { get; set; }
        public string Name { get; set; }
        public string PageId { get; set; }
        public ICollection<CodedRegionReference> IgnoreRegions { get; set; }
        public ICollection<CodedRegionReference> LayoutRegions { get; set; }
        public ICollection<CodedRegionReference> StrictRegions { get; set; }
        public ICollection<CodedRegionReference> ContentRegions { get; set; }
        public ICollection<TFloatingRegion> FloatingRegions { get; set; }
        public ICollection<TAccessibilityRegion> AccessibilityRegions { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }
        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public IDictionary<string, object> UfgOptions { get; set; }
        public LayoutBreakpointsOptions LayoutBreakpoints { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public int? RetryTimeout { get; set; }
        public ICollection<Renderer> Renders { get; set; } 
        public AutProxy AutProxy { get; set; }
        public Hooks Hooks { get; set; }
        public string UserCommandId { get; set; }

        public UniversalCheckSettings()
        {
            IgnoreRegions = new List<CodedRegionReference>();
            LayoutRegions = new List<CodedRegionReference>();
            StrictRegions = new List<CodedRegionReference>();
            ContentRegions = new List<CodedRegionReference>();
            FloatingRegions = new List<TFloatingRegion>();
            AccessibilityRegions = new List<TAccessibilityRegion>();
            Renders = new List<Renderer>();
        }
    }
}