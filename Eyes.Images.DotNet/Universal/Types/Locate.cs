using System;

namespace Applitools
{
    public class Locate : IEquatable<Locate>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public Locate(int top, int width, int height, int left)
        {
            Top = top;
            Width = width;
            Height = height;
            Left = left;
        }

        #region Overrides of Object

        public override string ToString()
        {
            return $"Left: {Left}, Top: {Top}, Width: {Width}, Height: {Height}";
        }

        #endregion

        #region Equality members

        public bool Equals(Locate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Left == other.Left && Top == other.Top && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Locate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Left;
                hashCode = (hashCode * 397) ^ Top;
                hashCode = (hashCode * 397) ^ Width;
                hashCode = (hashCode * 397) ^ Height;
                return hashCode;
            }
        }

        #endregion
    }
}