using System;
using System.Collections.Generic;
using Applitools.Fluent;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public interface ICheckConfig
    {
        string Name { get; set; }
        string PageId { get; set; }
        TRegion[] IgnoreRegions { get; set; }
        TRegion[] LayoutRegions { get; set; }
        TRegion[] StrictRegions { get; set; }
        TRegion[] ContentRegions { get; set; }
        TFloatingRegion[] FloatingRegions { get; set; }
        TFloatingRegion[] AccessibilityRegions { get; set; }
        AccessibilitySettings AccessibilitySettings { get; set; }
        MatchLevel? MatchLevel { get; set; }
        TimeSpan? RetryTimeout { get; }
        bool? SendDom { get; set; }
        bool? UseDom { get; set; }
        bool? EnablePatterns { get; set; }
        bool? IgnoreCaret { get; set; }
        IDictionary<string, object> UfgOptions { get; }
        ICollection<int> LayoutBreakpoints { get; set; }
        bool? DisableBrowserFetching { get; set; }
        AutProxy AutProxy { get; set; }
        ICollection<RenderBrowserInfo> Renderers { get; }
        Hooks Hooks { get; set; }
        string UserCommandId { get; set; }
    }

    public class CheckConfig : ICheckConfig
    {
        public string Name { get; set; }
        public string PageId { get; set; }
        public TRegion[] IgnoreRegions { get; set; }
        public TRegion[] LayoutRegions { get; set; }
        public TRegion[] StrictRegions { get; set; }
        public TRegion[] ContentRegions { get; set; }
        public TFloatingRegion[] FloatingRegions { get; set; }
        public TFloatingRegion[] AccessibilityRegions { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }
        public TimeSpan? RetryTimeout { get; set; }
        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public IDictionary<string, object> UfgOptions { get; set; }
        public ICollection<int> LayoutBreakpoints { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public AutProxy AutProxy { get; set; }
        public ICollection<RenderBrowserInfo> Renderers { get; set; }
        public Hooks Hooks { get; set; }
        public string UserCommandId { get; set; }
    }
}
