using Applitools.Utils.Geometry;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    public abstract class EmulationBaseInfo : IEquatable<EmulationBaseInfo>
    {
        protected EmulationBaseInfo(DeviceName deviceName, ScreenOrientation screenOrientation)
        {
            DeviceName = deviceName;
            ScreenOrientation = screenOrientation;
        }

        public DeviceName DeviceName { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ScreenOrientation ScreenOrientation { get; set; }
        public RectangleSize Size { get; set; }

        public bool Equals(EmulationBaseInfo other)
        {
            if (other == null) return false;
            return GetType() == other.GetType() && 
                   ScreenOrientation == other.ScreenOrientation;
        }
    }
}