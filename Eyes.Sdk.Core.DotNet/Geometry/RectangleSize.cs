using System;
using System.Drawing;

namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// The width and height of a rectangle.
    /// </summary>
    /// <remarks>The main motivation for having this class is to allow for proper Json 
    /// serialization</remarks>
    public class RectangleSize : IEquatable<RectangleSize>
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance.
        /// </summary>
        public RectangleSize() : this(0, 0)
        {
        }

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance of the input width and height.
        /// </summary>
        public RectangleSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance of the input size.
        /// </summary>
        public RectangleSize(Size size)
        {
            Width = size.Width;
            Height = size.Height;
        }

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance of the size of the input rectangle.
        /// </summary>
        public RectangleSize(Rectangle rectangle)
        {
            Width = rectangle.Width;
            Height = rectangle.Height;
        }

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance of the size of the input region.
        /// </summary>
        public RectangleSize(IRegion region)
        {
            ArgumentGuard.NotNull(region, nameof(region));

            Width = region.Width;
            Height = region.Height;
        }

        /// <summary>
        /// Creates a new <see cref="RectangleSize"/> instance.
        /// </summary>
        public RectangleSize(RectangleSize size)
        {
            ArgumentGuard.NotNull(size, nameof(size));

            Width = size.Width;
            Height = size.Height;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Region's width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Region's height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Returns <c>true</c> if both width and height are zero.
        /// </summary>
        public bool IsEmpty() { return Width == 0 && Height == 0; }

        #endregion

        #region Methods

        public RectangleSize Scale(double scaleRatio)
        {
            return new RectangleSize((int)Math.Round(Width * scaleRatio), (int)Math.Round(Height * scaleRatio));
        }

        /// <summary>
        /// Returns <c>true</c> if and only if the input objects are equal by value.
        /// </summary>
        public static bool AreEqual(RectangleSize obj1, RectangleSize obj2)
        {
            if (obj1 == obj2)
            {
                return true;
            }

            if (obj1 == null || obj2 == null)
            {
                return false;
            }

            return obj1.Width == obj2.Width && obj1.Height == obj2.Height;
        }

        /// <summary>
        /// Returns a <see cref="Size"/> representation of this object.
        /// </summary>
        public Size ToSize()
        {
            return new Size(Width, Height);
        }

        /// <summary>
        /// Returns a <see cref="Rectangle"/> representation of this object.
        /// </summary>
        public Rectangle ToRectangle()
        {
            return new Rectangle(0, 0, Width, Height);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public bool Equals(RectangleSize other)
        {
            if (other == null) return false;
            return other.Width == Width && other.Height == Height;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RectangleSize);
        }

        public override int GetHashCode()
        {
            return Height * 10000 + Width;
        }

        public static implicit operator RectangleSize(Size size)
        {
            return new RectangleSize(size);
        }

        public static implicit operator Size(RectangleSize size)
        {
            return new Size(size.Width, size.Height);
        }
        #endregion
    }
}
