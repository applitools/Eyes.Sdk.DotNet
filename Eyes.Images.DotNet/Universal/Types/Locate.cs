namespace Applitools
{
    public class Locate
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
    }
}