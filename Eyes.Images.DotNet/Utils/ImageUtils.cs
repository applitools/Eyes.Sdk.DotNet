using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace Applitools.Utils
{
    public static class ImageUtils
    {
        public static Bitmap GetImageFromFile(string path)
        {
            return new Bitmap(path);
        }

        public static Bitmap GetImageFromUrl(string url)
        {
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            var bitmap = new Bitmap(responseStream);

            return bitmap;
        }

        public static string ToBase64(this Bitmap image)
        {
            var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            var base64 = Convert.ToBase64String(byteImage);

            return base64;
        }

        public static string ToBase64(this string filePath)
        {
            var bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }
    }
}
