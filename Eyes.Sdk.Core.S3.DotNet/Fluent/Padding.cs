namespace Applitools.Fluent
{
    public class Padding
    {
        public int Left  { get; }
        public int Top  { get; }
        public int Right  { get; }
        public int Bottom  { get; }

        public Padding(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}