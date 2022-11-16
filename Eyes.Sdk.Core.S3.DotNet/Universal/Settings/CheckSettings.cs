using System.Collections.Generic;
using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.Commands
{
    public class CheckSettings : SettingsBase
    {
        public bool? IgnoreDisplacements { get; set; }
        public string Name { get; set; }
        public string PageId { get; set; }
        public CodedRegionReference[] IgnoreRegions { get; set; }
        public CodedRegionReference[] LayoutRegions { get; set; }
        public CodedRegionReference[] StrictRegions { get; set; }
        public CodedRegionReference[] ContentRegions { get; set; }
        public TFloatingRegion[] FloatingRegions { get; set; }
        public TAccessibilityRegion[] AccessibilityRegions { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }
        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public IDictionary<string, object> UfgOptions { get; set; }
        public object LayoutBreakpoints { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public int? RetryTimeout { get; set; }
        public ICollection<Renderer> Renders { get; set; } 
        public AutProxy AutProxy { get; set; }
        public Hooks Hooks { get; set; }
        public string UserCommandId { get; set; }
    }
}