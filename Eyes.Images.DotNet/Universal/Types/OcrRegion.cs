using System.Drawing;
using Applitools.Utils;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Images
{
    public class OcrRegion : OcrRegionBase
    {
        private readonly string _image;
        private readonly Bitmap _bitmapImage;
        private readonly Region? _region;
			
        public OcrRegion(Bitmap image, Region? region = null)
        {
	        _bitmapImage = image;
	        _region = region;
        }
        
		public OcrRegion(string image, Region? region = null)
		{
			_image = image;
			_region = region;
		}

		public string GetImage()
        {
	        return _bitmapImage != null ? _bitmapImage.ToBase64() : _image;
        }

		public Region? GetRegion()
        {
	        return _region;
        }
    }
}