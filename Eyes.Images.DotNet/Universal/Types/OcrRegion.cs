using Applitools.Utils.Geometry;

namespace Applitools
{
    public class OcrRegion
    {
        private readonly string _image;
        private readonly Region _region;

        public OcrRegion(string image)
        {
            _image = image;
        }

        public OcrRegion(string image, Region region)
        {
            _image = image;
            _region = region;
        }
    }
}