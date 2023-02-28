using System;

namespace Applitools.Commands
{
    public class TextRegion : UniversalRegion, IEquatable<TextRegion>
    {
        public string Text { get; set; }

        public TextRegion(int x, int y, int height, int width, string text)
        {
            X = x;
            Y = y;
            Height = height;
            Width = width;
            Text = text;
        }

        #region Equality members

        public bool Equals(TextRegion other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Text == other.Text && other.X == X && other.Y == Y && other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TextRegion) obj);
        }

        public override int GetHashCode()
        {
            return (Text != null ? Text.GetHashCode() : 0) + X + Y + Width + Height;
        }

        #endregion
    }
}