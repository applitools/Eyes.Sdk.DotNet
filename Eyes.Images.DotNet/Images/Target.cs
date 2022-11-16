using System;
using System.Drawing;

namespace Applitools.Images
{
    public static class Target
    {
        public static ImagesCheckSettings Image(Bitmap image)
        {
            return new ImagesCheckSettings(image);
        }

        public static ImagesCheckSettings Image(string path)
        {
            Bitmap image = new Bitmap(path);
            return new ImagesCheckSettings(image);
        }

        public static ImagesCheckSettings Url(Uri uri)
        {
            return new ImagesCheckSettings(uri);
        }

        public static ImagesCheckSettings Url(string uriString)
        {
            Uri uri = new Uri(uriString);
            return new ImagesCheckSettings(uri);
        }
    }
}
