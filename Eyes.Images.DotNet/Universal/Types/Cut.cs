namespace Applitools
{
    public class Cut
    {
        public int Top { get; }
        public int Right { get; }
        public int Bottom { get; }
        public int Left { get; }

        public Cut(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
    }
}