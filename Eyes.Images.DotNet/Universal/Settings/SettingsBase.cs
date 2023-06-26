using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public abstract class SettingsBase
    {
        public TRegion Region { get; set; }
        public ICollection<IFrame> Frames { get; set; }
        public object Webview { get; set; }
        public bool? Fully { get; set; }
        public TRegion ScrollRootElement { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StitchModes? StitchMode { get; set; }
        public bool? HideScrollbars { get; set; }
        public bool? HideCaret { get; set; }
        public Overlap Overlap { get; set; }
        public int? WaitBeforeCapture { get; set; }
        public LazyLoadOptions LazyLoad { get; set; }
        public Normalization Normalization { get; set; }
        public DebugImages DebugImages { get; set; }

        protected SettingsBase()
        {
            Frames = new List<IFrame>();
        }
    }
}