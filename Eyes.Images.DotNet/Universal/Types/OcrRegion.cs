using System.Drawing;
using Applitools.Utils;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    public class OcrRegion : OcrRegionBase
    {
        private readonly string image_;
        private readonly Bitmap bitmapImage_;
        private readonly Region? region_;
			
        public OcrRegion(Bitmap image, Region? region = null)
        {
	        bitmapImage_ = image;
	        region_ = region;
        }
        
		public OcrRegion(string image, Region? region = null)
		{
			image_ = image;
			region_ = region;
		}

		public string GetImage()
        {
	        return bitmapImage_ != null ? bitmapImage_.ToBase64() : image_;
        }

		public Region? GetRegion()
        {
	        return region_;
        }
    }
}