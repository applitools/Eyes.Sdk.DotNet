using Applitools.Selenium;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    public class IosDeviceInfo : IRenderBrowserInfo, IEquatable<IosDeviceInfo>
    {
        public IosDeviceInfo
            (IosDeviceName deviceName,
            ScreenOrientation screenOrientation = Applitools.VisualGrid.ScreenOrientation.Portrait,
            IosVersion? iosVersion = null)
        {
            DeviceName = deviceName;
            ScreenOrientation = screenOrientation;
            Version = iosVersion;
        }

        [JsonProperty("name")]
        public IosDeviceName DeviceName { get; }

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

        [JsonProperty("screenOrientation")]
        public ScreenOrientation ScreenOrientation { get; }
        
        [JsonProperty("version")]
        public IosVersion? Version { get; }

        public bool Equals(IosDeviceInfo other)
        {
            if (other == null) return false;

            return DeviceName == other.DeviceName &&
                ScreenOrientation == other.ScreenOrientation;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IosDeviceInfo);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(IosDeviceInfo)} {{{DeviceName} {Version} {ScreenOrientation}}}";
        }
    }
}
