namespace Applitools.Utils.Geometry
{
    using System;
    using System.Drawing;

    /// <summary>
    /// A location in a 2D plane.
    /// </summary>
    public class Location : IEquatable<Location>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="Location"/> instance.
        /// </summary>
        public Location(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Creates a new <see cref="Location"/> instance.
        /// </summary>
        public Location(Point point)
        {
            X = point.X;
            Y = point.Y;
        }

        /// <summary>
        /// Creates a new <see cref="Location"/> instance.
        /// </summary>
        public Location() : this(0, 0)
        {
        }

        public Location Offset(int x, int y)
        {
            return new Location(this.X + x, this.Y + y);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Horizontal position (on the X axis)
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Vertical position (on the Y axis).
        /// </summary>
        public int Y { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns this location as a <see cref="System.Drawing.Point"/>
        /// </summary>
        public Point ToPoint()
        {
            return new Point(X, Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public bool Equals(Location other)
        {
            if (other == null) return false;
            return X == other.X && Y == other.Y;
        }

        public static implicit operator Location(Point point)
        {
            return new Location(point);
        }

        #endregion
    }
}
