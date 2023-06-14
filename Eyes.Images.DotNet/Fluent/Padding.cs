namespace Applitools
{
    public class Padding
    {
        public int Left  { get; }
        public int Top  { get; }
        public int Right  { get; }
        public int Bottom  { get; }

        public Padding(int left = 0, int top = 0, int right = 0, int bottom = 0)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public Padding(int all) : this(all,all,all,all)
        {
        }

    }
}