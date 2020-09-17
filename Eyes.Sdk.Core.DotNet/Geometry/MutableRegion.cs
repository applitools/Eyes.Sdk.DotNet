namespace Applitools.Utils.Geometry
{
    using Newtonsoft.Json;
    using System;
    using System.Drawing;

    /// <summary>
    /// A basic mutable region implementation.
    /// </summary>
    public class MutableRegion : IMutableRegion
    {
        #region Constructors

        /// <summary>
        /// Creates a new empty <see cref="MutableRegion"/> instance.
        /// </summary>
        public MutableRegion()
        {
        }

        /// <summary>
        /// Creates a new <see cref="MutableRegion"/> instance of the input position and size.
        /// </summary>
        public MutableRegion(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Creates a new <see cref="MutableRegion"/> instance of the input position and size.
        /// </summary>
        public MutableRegion(Rectangle rectangle)
            : this(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height)
        {
        }

        /// <summary>
        /// Creates a new <see cref="MutableRegion"/> instance of the input size.
        /// </summary>
        public MutableRegion(Size size) 
            : this(0, 0, size.Width, size.Height)
        {
        }

        /// <summary>
        /// Creates a new <see cref="MutableRegion"/> instance of the input region.
        /// </summary>
        public MutableRegion(IRegion region) : this(
            ArgumentGuard.NotNull(region, nameof(region)).Left, 
            region.Top, 
            region.Width, 
            region.Height)
        {
        }

        /// <summary>
        /// Creates a new <see cref="MutableRegion"/> instance at the input offset from the 
        /// specified region.
        /// </summary>
        public MutableRegion(IRegion region, Point offset) : this(
            ArgumentGuard.NotNull(region, nameof(region)).Left + offset.X,
            region.Top + offset.Y,
            region.Width,
            region.Height)
        {
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        [JsonProperty]
        public int Left
        {
            get;
            set;
        }

        /// <inheritdoc />
        [JsonProperty]
        public int Top
        {
            get;
            set;
        }

        /// <inheritdoc />
        [JsonProperty]
        public int Width
        {
            get;
            set;
        }

        /// <inheritdoc />
        [JsonProperty]
        public int Height
        {
            get;
            set;
        }

        /// <inheritdoc />
        [JsonIgnore]
        public int Right
        {
            get { return Left + Width; }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public int Bottom
        {
            get { return Top + Height; }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public Point Location
        {
            get 
            { 
                return new Point(Left, Top); 
            }

            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public Size Size
        {
            get 
            { 
                return new Size(Width, Height); 
            }

            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public int Area
        {
            get { return Width * Height; }
        }

        /// <inheritdoc />
        [JsonIgnore]
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle(Location, Size);
            }

            set
            {
                Left = value.Left;
                Top = value.Top;
                Width = value.Width;
                Height = value.Height;
            }
        }

        /// <summary>
        /// Gets the center of this region.
        /// </summary>
        [JsonIgnore]
        public Location Center
        {
            get { return new Location(Left + (Width / 2), Top + (Height / 2)); }
        }

        /// <summary>
        /// Offsets this instance by a given amount of pixels.
        /// </summary>
        /// <param name="x">Amount of pixels to move the region horizontally.</param>
        /// <param name="y">Amount of pixels to move the region vertically.</param>
        public void Offset(int x, int y)
        {
            this.Left += x;
            this.Top += y;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public override string ToString()
        {
            return "{0},{1} {2}x{3}".Fmt(Left, Top, Width, Height);
        }

        Region IRegion.Offset(int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
