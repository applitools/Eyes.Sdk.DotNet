namespace Applitools
{
    public interface IImageCrop
    {

    }

    public class ImageCropRect : IImageCrop
    {
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
        public int Left { get; set; }

        public ImageCropRect(int top, int right, int bottom, int left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
    }
}
