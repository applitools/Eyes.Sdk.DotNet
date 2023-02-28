using Applitools;
using Applitools.Commands;
using Applitools.Images;
using Applitools.Utils.Geometry;

namespace Eyes.Images.Core.E2ETests
{
    public class ImagesGenericTests : ImagesGenericTestsBase
    {
        private static string IMAGE_PATH_JPEG = "resources/minions_jpeg.jpeg";
        private static string IMAGE_PATH_PNG = "resources/minions_png.png";
        private static string IMAGE_PATH_BMP = "resources/minions_bitmap.bmp";
        private static string EXTRACT_IMAGE_PATH_PNG = "resources/extractText.png";
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
        
        [Test]
        public void ExtractTextTest()
        {
            eyes.Open("Applitools Eyes SDK", "ExtractText", new RectangleSize(ImageWidth, ImageHeight));
            var result = eyes.ExtractText(new OcrRegion(EXTRACT_IMAGE_PATH_PNG));
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("This is the navigation bar", result.ElementAt(0));
            eyes.Close();
        }

        [Test]
        public void ExtractTextRegionsTest()
        {
            var image = EXTRACT_IMAGE_PATH_PNG;
            eyes.Open("Applitools Eyes SDK", "ExtractTextRegions");
            var result = eyes.ExtractTextRegions(new TextRegionSettings(".+").SetImage(image));
            Assert.AreEqual(1, result.Count);
            var regions = result.First().Value;
            Assert.AreEqual(1, regions.Count);
            var region = regions.First();
            Assert.AreEqual(new TextRegion(10, 11, 18, 214, "Thisisthenavigationbar"), region);
            eyes.Close();
        }
    }
}