using System.Collections.Generic;
using Applitools.Commands;
using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public interface IScreenshotConfig
    {
        TRegion Region { get; }
        ICollection<IFrame> Frames { get; }
        bool? Fully { get; }
        TRegion ScrollRootElement { get; }
        StitchModes? StitchMode { get; }
        bool? HideScrollbars { get; }
        bool? HideCaret { get; }
        Overlap Overlap { get; }
        int? WaitBeforeCapture { get; }
        LazyLoadOptions LazyLoad { get; }
        bool? IgnoreDisplacements { get; }
        Normalization Normalization { get; }
        DebugImages DebugImages { get; }
    }

    public class ScreenshotConfig : IScreenshotConfig
    {
        public TRegion Region { get; set; }
        public ICollection<IFrame> Frames { get; set; }
        public bool? Fully { get; set; }
        public TRegion ScrollRootElement { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StitchModes? StitchMode { get; set; }
        public bool? HideScrollbars { get; set; }
        public bool? HideCaret { get; set; }
        public Overlap Overlap { get; set; }
        public int? WaitBeforeCapture { get; set; }
        public LazyLoadOptions LazyLoad { get; set; }
        public bool? IgnoreDisplacements { get; set; }
        public Normalization Normalization { get; set; }
        public DebugImages DebugImages { get; set; }
    }
}