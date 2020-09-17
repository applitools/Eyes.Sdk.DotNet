using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools
{
    public class AccessibilityRegionByRectangle : IGetAccessibilityRegion, IEquatable<AccessibilityRegionByRectangle>
    {
        public AccessibilityRegionByRectangle() { }

        public AccessibilityRegionByRectangle(int left, int top, int width, int height, AccessibilityRegionType regionType)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
            Type = regionType;
        }

        public AccessibilityRegionByRectangle(Rectangle region, AccessibilityRegionType regionType)
        {
            Region = region;
            Type = regionType;
        }

        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        [JsonIgnore]
        public Rectangle Region
        {
            get
            {
                return new Rectangle(Left, Top, Width, Height);
            }
            set
            {
                Left = value.X;
                Top = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public AccessibilityRegionType Type { get; set; }

        public IList<AccessibilityRegionByRectangle> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot)
        {
            return new AccessibilityRegionByRectangle[] { this };
        }

        public bool Equals(AccessibilityRegionByRectangle other)
        {
            return other != null && 
                Left.Equals(other.Left) && Top.Equals(other.Top) && 
                Width.Equals(other.Width) && Height.Equals(other.Height) && Type.Equals(other.Type);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AccessibilityRegionByRectangle);
        }

        public override int GetHashCode()
        {
            return Top + Left * 100 + Width * 10000 + Height * 1000000;
        }

        public override string ToString()
        {
            return $"{nameof(AccessibilityRegionByRectangle)} ({Left}, {Top}) {Width}x{Height} - {Type}";
        }
    }
}