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
        ICollection<TRegion> IgnoreRegions { get; set; }
        ICollection<TRegion> LayoutRegions { get; set; }
        ICollection<TRegion> StrictRegions { get; set; }
        ICollection<TRegion> ContentRegions { get; set; }
        ICollection<TFloatingRegion> FloatingRegions { get; set; }
        ICollection<TFloatingRegion> AccessibilityRegions { get; set; }
        AccessibilitySettings AccessibilitySettings { get; set; }
        MatchLevel? MatchLevel { get; set; }
        TimeSpan? RetryTimeout { get; }
        bool? SendDom { get; set; }
        bool? UseDom { get; set; }
        bool? EnablePatterns { get; set; }
        bool? IgnoreCaret { get; set; }
        IDictionary<string, object> UfgOptions { get; }
        LayoutBreakpointsOptions LayoutBreakpoints { get; set; }
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
        public ICollection<TRegion> IgnoreRegions { get; set; }
        public ICollection<TRegion> LayoutRegions { get; set; }
        public ICollection<TRegion> StrictRegions { get; set; }
        public ICollection<TRegion> ContentRegions { get; set; }
        public ICollection<TFloatingRegion> FloatingRegions { get; set; }
        public ICollection<TFloatingRegion> AccessibilityRegions { get; set; }
        public AccessibilitySettings AccessibilitySettings { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MatchLevel? MatchLevel { get; set; }

        [JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan? RetryTimeout { get; set; }

        public bool? SendDom { get; set; }
        public bool? UseDom { get; set; }
        public bool? EnablePatterns { get; set; }
        public bool? IgnoreCaret { get; set; }
        public IDictionary<string, object> UfgOptions { get; set; }
        public LayoutBreakpointsOptions LayoutBreakpoints { get; set; }
        public bool? DisableBrowserFetching { get; set; }
        public AutProxy AutProxy { get; set; }
        public ICollection<RenderBrowserInfo> Renderers { get; set; }
        public Hooks Hooks { get; set; }
        public string UserCommandId { get; set; }

        public CheckConfig()
        {
            IgnoreRegions = new List<TRegion>();
            LayoutRegions = new List<TRegion>();
            StrictRegions = new List<TRegion>();
            ContentRegions = new List<TRegion>();
            FloatingRegions = new List<TFloatingRegion>();
            AccessibilityRegions = new List<TFloatingRegion>();
            UfgOptions = new Dictionary<string, object>();
            LayoutBreakpoints = new LayoutBreakpointsOptions();
            Renderers = new List<RenderBrowserInfo>();
        }
    }

    public class TimeSpanConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            if (value is TimeSpan timeSpan)
            {
                writer.WriteValue(timeSpan.Milliseconds);
                return;
            }

            throw new JsonSerializationException($"can't serialize {value} to TimeSpan");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonToken.String)
            {
                return TimeSpan.Parse(reader.Value?.ToString());
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                return TimeSpan.FromMilliseconds((int)reader.Value); 
            }

            throw new JsonSerializationException($"can't deserialize TimeSpan from {reader.Value}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(int) || objectType == typeof(int?);
        }
    }
}