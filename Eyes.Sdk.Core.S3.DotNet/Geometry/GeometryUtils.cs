namespace Applitools.Utils.Geometry
{
    using System.Drawing;
    
    /// <summary>
    /// Geometry related utilities.
    /// </summary>
    public static class GeometryUtils
    {
        #region Methods

        #region Rectangle
        
        /// <summary>
        /// Returns a string representation of the input rectangle.
        /// </summary>
        public static string ToString(Rectangle rectangle)
        {
            return $"({rectangle.X}, {rectangle.Y}) {rectangle.Width}x{rectangle.Height}";
        }
        
        #endregion

        #region Size
        
        /// <summary>
        /// Returns the area of the input size.
        /// </summary>
        public static int Area(this Size size)
        {
            return size.Width * size.Height;
        }
 
        #endregion
        
        #endregion
    }
}
