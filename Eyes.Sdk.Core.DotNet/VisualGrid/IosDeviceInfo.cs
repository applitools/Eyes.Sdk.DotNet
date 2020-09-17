using Applitools.Selenium;
using Applitools.Utils;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    public class IosDeviceInfo : IRenderBrowserInfo, IEquatable<IosDeviceInfo>
    {
        public IosDeviceInfo
            (IosDeviceName deviceName,
            ScreenOrientation screenOrientation = Applitools.VisualGrid.ScreenOrientation.Portrait)
        {
            DeviceName = deviceName;
            ScreenOrientation = screenOrientation;
        }

        [JsonProperty("name")]
        public IosDeviceName DeviceName { get; }

        [JsonIgnore]
        public string SerializedDeviceName
        {
            get
            {
                try { return DeviceName.GetAttribute<EnumMemberAttribute>().Value; }
                catch (Exception) { return DeviceName.ToString(); }
            }
        }

        public ScreenOrientation ScreenOrientation { get; }
        public string BaselineEnvName { get; }

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
            return $"{nameof(IosDeviceInfo)} {{{DeviceName} {ScreenOrientation}}}";
        }
    }
}
