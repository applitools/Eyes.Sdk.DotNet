namespace Applitools.Commands
{
    public class TextRegion : UniversalRegion
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
    }
}