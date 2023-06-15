using System;
using System.Linq;
using Newtonsoft.Json;

namespace Applitools.Utils
{
    public class LayoutBreakpointsConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            object breakpoints;
            LayoutBreakpointsOptions bpOptions = (LayoutBreakpointsOptions)value;
            var breakpointsList = bpOptions.GetLayoutBreakpoints();
            if (breakpointsList?.Any() ?? false)
            {
                breakpoints = breakpointsList;
            }
            else
            {
                breakpoints = bpOptions.IsLayoutBreakpoints();
            }

            object dto = new { breakpoints, reload = bpOptions.GetReload() };
            serializer.Serialize(writer, dto);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LayoutBreakpointsOptions);
        }
    }
}