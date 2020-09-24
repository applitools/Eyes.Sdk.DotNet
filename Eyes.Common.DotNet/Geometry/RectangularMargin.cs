namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// The inner or outer margins of a rectangular element.
    /// </summary>
    public struct RectangularMargins
    {
        #region Constructors
        
        /// <summary>
        /// Creates a new <see cref="RectangularMargins"/> instance.
        /// </summary>
        public RectangularMargins(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the left edge margin value.
        /// </summary>
        public int Left
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the top edge margin value.
        /// </summary>
        public int Top
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the right edge margin value.
        /// </summary>
        public int Right
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the bottom edge margin value.
        /// </summary>
        public int Bottom
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the margin value for all edges. Returns <c>-1</c> if not all edges
        /// have the same padding value.
        /// </summary>
        public int All
        {
            get
            {
                return Left == Top && Top == Right && Right == Bottom ? Left : -1;
            }

            set
            {
                Left = Top = Right = Bottom = value;
            }
        }

        /// <summary>
        /// Gets the combined margin of the left and right edges.
        /// </summary>
        public int Horizontal
        {
            get
            {
                return Left + Right;
            }
        }

        /// <summary>
        /// Gets the combined margin of the top and bottom edges.
        /// </summary>
        public int Vertical
        {
            get
            {
                return Top + Bottom;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the <see cref="RectangularMargins"/> object encoded in the input string
        /// (&lt;left&gt;, &lt;top&gt;, &lt;right&gt; ,&lt;bottom&gt;).
        /// </summary>
        public static RectangularMargins Parse(string value)
        {
            ArgumentGuard.NotNull(value, nameof(value));

            var parts = value.Split(',');
            return new RectangularMargins()
            {
                Left = parts[0].ToInt32(),
                Top = parts[1].ToInt32(),
                Right = parts[2].ToInt32(),
                Bottom = parts[3].ToInt32()
            };
        }

        public override string ToString()
        {
            return $"L:{Left}, T:{Top}, R:{Right}, B:{Bottom}";
        }
        #endregion
    }
}
