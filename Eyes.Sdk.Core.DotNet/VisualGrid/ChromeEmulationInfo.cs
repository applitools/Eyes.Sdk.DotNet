using Applitools.Selenium;
using Applitools.Utils;
using System;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    public class ChromeEmulationInfo : EmulationBaseInfo, IRenderBrowserInfo, IEquatable<ChromeEmulationInfo>
    {
        public string SerializedDeviceName
        {
            get
            {
                try { return DeviceName.GetAttribute<EnumMemberAttribute>().Value; }
                catch (Exception) { return DeviceName.ToString(); }
            }
        }

        public ChromeEmulationInfo(
            DeviceName deviceName,
            ScreenOrientation screenOrientation = ScreenOrientation.Portrait)
            : base(deviceName, screenOrientation)
        {
            
        }

        public override string ToString()
        {
            return $"{DeviceName} ({ScreenOrientation})";
        }

        public bool Equals(ChromeEmulationInfo other)
        {
            if (other == null) return false;
            return base.Equals(other) && DeviceName == other.DeviceName;
        }
    }
}