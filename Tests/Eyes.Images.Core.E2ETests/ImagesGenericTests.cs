using Applitools.Images;
using Applitools.Utils.Geometry;

namespace Eyes.Images.Core.E2ETests
{
    public class ImagesGenericTests : ImagesGenericTestsBase
    {
        private static string IMAGE_PATH_JPEG = "resources/minions_jpeg.jpeg";
        private static string IMAGE_PATH_PNG = "resources/minions_png.png";
        private static string IMAGE_PATH_BMP = "resources/minions_bitmap.bmp";
        private const int ImageWidth = 800;
        private const int ImageHeight = 500;
        
        [Test]
        public void BitMapFromBufferTest()
        {
            var image = File.ReadAllBytes(IMAGE_PATH_BMP);
            eyes.Open("Applitools Eyes SDK", "TestBitMapFromBuffer - Core", new RectangleSize(ImageWidth, ImageHeight));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }
        
        [Test]
        public void JPEGFromPathTest()
        {
            eyes.Open("Applitools Eyes SDK", "TestJPEGFromPath - Core", new RectangleSize(ImageWidth, ImageHeight));
            eyes.Check(Target.Image(IMAGE_PATH_JPEG));
            eyes.Close();
        }
        
        [Test]
        public void JPEGFromBufferTest()
        {
            var image = File.ReadAllBytes(IMAGE_PATH_JPEG);
            eyes.Open("Applitools Eyes SDK", "TestJPEGFromBuffer - Core", new RectangleSize(ImageWidth, ImageHeight));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }
        
        [Test]
        public void PNGFromBufferTest()
        {
            var image = File.ReadAllBytes(IMAGE_PATH_PNG);
            eyes.Open("Applitools Eyes SDK", "TestPNGFromBuffer - Core", new RectangleSize(ImageWidth, ImageHeight));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }
    }
}