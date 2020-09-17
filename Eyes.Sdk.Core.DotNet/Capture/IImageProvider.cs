using System.Drawing;

namespace Applitools.Capture
{
    public interface IImageProvider
    {
        Bitmap GetImage();
    }
}