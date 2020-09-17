using System.Drawing;
using Applitools.Utils.Geometry;

namespace Applitools.VisualGrid
{
    public class VGRegion : IRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Error { get; set; }

        public Location Location => new Location(X, Y);

        public int Left => X;

        public int Right => X + Width;

        public int Top => Y;

        public int Bottom => Y + Height;

        public Size Size => new Size(Width, Height);

        public int Area => Width * Height;

        public Rectangle Rectangle => new Rectangle(X, Y, Width, Height);

        Point IRegion.Location => new Point(X,Y);

        public Utils.Geometry.Region Offset(int x, int y)
        {
            Rectangle r = Rectangle;
            r.Offset(x, y);
            return new Utils.Geometry.Region(r);
        }

        public Utils.Geometry.Region ToRegion()
        {
            if (Error != null)
            {
                throw new EyesException(Error);
            }

            return new Utils.Geometry.Region(X, Y, Width, Height);
        }

        public override string ToString()
        {
            return Error == null ? $"({X},{Y}) [{Width}x{Height}]" : $"Error: {Error}";
        }
    }
}