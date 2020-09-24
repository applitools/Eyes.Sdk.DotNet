using System;

namespace Applitools
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class FloatingMatchSettings : IEquatable<FloatingMatchSettings>
    {
        // default ctor for deserialization.
        public FloatingMatchSettings() { }

        public FloatingMatchSettings(int left, int top, int width, int height, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;

            MaxUpOffset = maxUpOffset;
            MaxDownOffset = maxDownOffset;
            MaxLeftOffset = maxLeftOffset;
            MaxRightOffset = maxRightOffset;
        }

        public int Top { get; set; }
        public int Left { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public int MaxUpOffset { get; set; }
        public int MaxDownOffset { get; set; }
        public int MaxLeftOffset { get; set; }
        public int MaxRightOffset { get; set; }

        public bool Equals(FloatingMatchSettings other)
        {
            return other != null && 
                Top == other.Top && Left == other.Left && Width == other.Width && Height == other.Height &&
                MaxUpOffset == other.MaxUpOffset && MaxDownOffset == other.MaxDownOffset &&
                MaxLeftOffset == other.MaxLeftOffset && MaxRightOffset == other.MaxRightOffset;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FloatingMatchSettings);
        }

        public override int GetHashCode()
        {
            return Top + Left * 100 + Width * 10000 + Height * 1000000;
        }

        public override string ToString()
        {
            return string.Format("bounds: ({0},{1}) {2}x{3} ; max offsets: u:{4} d:{5} l:{6} r:{7}",
                Left, Top, Width, Height, MaxUpOffset, MaxDownOffset, MaxLeftOffset, MaxRightOffset);
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

}
