using Applitools.Utils.Images;
using System;
using System.Drawing;

namespace Applitools.Capture
{
    class SimpleImageProvider : IImageProvider, IDisposable
    {
        private Bitmap image_;

        private SimpleImageProvider(Bitmap image)
        {
            image_ = image;
        }

        public static SimpleImageProvider FromBytes(byte[] screenshotBytes) {
            Bitmap bmp = BasicImageUtils.CreateBitmap(screenshotBytes);
            return new SimpleImageProvider(bmp);
        }

        public void Dispose()
        {
            if (image_ != null)
            {
                image_.Dispose();
                image_ = null;
            }
        }

        public Bitmap GetImage()
        {
            return image_;
        }
    }
}
