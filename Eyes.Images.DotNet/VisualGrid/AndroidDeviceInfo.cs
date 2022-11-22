using System;
using System.Runtime.Serialization;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools.VisualGrid
{
    public class AndroidDeviceInfo : IRenderBrowserInfo, IEquatable<AndroidDeviceInfo>
    {
        public AndroidDeviceInfo
        (AndroidDeviceName deviceName,
            ScreenOrientation screenOrientation = ScreenOrientation.Portrait,
            AndroidVersion? androidVersion = null)
        {
            DeviceName = deviceName;
            ScreenOrientation = screenOrientation;
            Version = androidVersion;
        }

        public AndroidDeviceName DeviceName { get; }

        [JsonIgnore]
        public RectangleSize Size { get; set; }

        [JsonIgnore]
        public string SerializedDeviceName
        {
            get
            {
                try { return DeviceName.GetAttribute<EnumMemberAttribute>().Value; }
                catch (Exception) { return DeviceName.ToString(); }
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("screenOrientation")]
        public ScreenOrientation ScreenOrientation { get; }
        
        [JsonProperty("version")]
        public AndroidVersion? Version { get; }

        public bool Equals(AndroidDeviceInfo other)
        {
            if (other == null) return false;

            return DeviceName == other.DeviceName &&
                   ScreenOrientation == other.ScreenOrientation;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AndroidDeviceInfo);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(AndroidDeviceInfo)} {{{DeviceName} {Version} {ScreenOrientation}}}";
        }
    }
}