using Applitools.Selenium;
using Applitools.Utils;
using System;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    public class ChromeEmulationInfo : EmulationBaseInfo, IRenderBrowserInfo
    {
        public DeviceName DeviceName { get; set; }
        public string SerializedDeviceName
        {
            get
            {
                try { return DeviceName.GetAttribute<EnumMemberAttribute>().Value; }
                catch (Exception) { return DeviceName.ToString(); }
            }
        }

        public ChromeEmulationInfo(DeviceName deviceName, ScreenOrientation screenOrientation) : base(screenOrientation)
        {
            DeviceName = deviceName;
        }

        public override string ToString()
        {
            return $"{DeviceName} ({ScreenOrientation})";
        }
    }
}