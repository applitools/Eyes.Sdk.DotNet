using Applitools.Utils;
using Newtonsoft.Json;
using System;
using System.Drawing;

namespace Applitools.VisualGrid
{
    public class RenderInfo : IEquatable<RenderInfo>
    {
        public RenderInfo(int width, int height, SizeMode target, VisualGridSelector selector,
            Rectangle? region, EmulationBaseInfo emulationInfo, IosDeviceInfo iosDeviceInfo)
        {
            Width = width;
            Height = height;
            Target = target;
            Selector = selector;
            Region = region;
            EmulationInfo = emulationInfo;
            IosDeviceInfo = iosDeviceInfo;
        }

        public RenderInfo() { }

        public int Width { get; set; }
        public int Height { get; set; }

        [JsonConverter(typeof(RectangleConverterForVG))]
        public Rectangle? Region { get; set; }

        public SizeMode Target { get; set; }
        public VisualGridSelector Selector { get; set; }
        public EmulationBaseInfo EmulationInfo { get; set; }
        public IosDeviceInfo IosDeviceInfo { get; set; }

        public bool Equals(RenderInfo other)
        {
            if (other == null) return false;
            return Width == other.Width &&
                   Height == other.Height &&
                   Region == other.Region &&
                   Target == other.Target &&
                   Selector == other.Selector &&
                   EmulationInfo == other.EmulationInfo &&
                   IosDeviceInfo == other.IosDeviceInfo;
        }
    }
}