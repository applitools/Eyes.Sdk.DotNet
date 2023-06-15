using System;
using System.Drawing;
using System.Linq;
using Applitools;
using Applitools.Commands;
using Applitools.Images;
using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using Region = Applitools.Utils.Geometry.Region;

namespace Eyes.Images.E2ETests
{
    public class ImagesGenericTests : ImagesGenericTestsBase
    {
        private static string IMAGE_URL_JPG = "https://applitools.github.io/demo/images/image_1.jpg";
        private static string IMAGE_URL_JPEG = "https://applitools.github.io/demo/images/minions_jpeg.jpeg";
        private static string IMAGE_URL_PNG = "https://applitools.github.io/upload/appium.png";
        private static string IMAGE_URL_BMP = "https://applitools.github.io/demo/images/minions_bitmap.bmp";
        private static string EXTRACT_TEXT_IMAGE_PNG = "resources/extractText.png";

        private static string IMAGE_PATH_JPEG = "resources/minions_jpeg.jpeg";
        private static string IMAGE_PATH_JPG = "";
        private static string IMAGE_PATH_PNG = "resources/minions_png.png";
        private static string IMAGE_PATH_BMP = "resources/minions_bitmap.bmp";

        private Bitmap TEST_IMAGE = new Bitmap(IMAGE_PATH_JPEG);
        private Bitmap EXTRACT_TEXT_IMAGE = new Bitmap(EXTRACT_TEXT_IMAGE_PNG);
        
        [Test]
        public void BitMapFromPathTest()
        {
            var image = new Bitmap(IMAGE_PATH_BMP);
            eyes.Open("Applitools Eyes SDK", "TestBitMapFromPath", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(IMAGE_PATH_BMP));
            eyes.Close();
        }
        
        [Test]
        public void BitMapFromBufferTest()
        {
            var image = new Bitmap(IMAGE_PATH_BMP);
            eyes.Open("Applitools Eyes SDK", "TestBitMapFromBuffer", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }
        
        [Test]
        public void BitMapFromURLTest()
        {
            eyes.Open("Applitools Eyes SDK", "TestBitMapFromURL");
            eyes.Check(Target.Url(IMAGE_URL_BMP));
            eyes.Close();
        }
        
        [Test]
        public void JPEGFromPathTest()
        {
            var image = new Bitmap(IMAGE_PATH_JPEG);
            eyes.Open("Applitools Eyes SDK", "TestJPEGFromPath", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(IMAGE_PATH_JPEG));
            eyes.Close();
        }
        
        [Test]
        public void JPEGFromBufferTest()
        {
            var image = new Bitmap(IMAGE_PATH_JPEG);
            eyes.Open("Applitools Eyes SDK", "TestJPEGFromBuffer", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }
        
        [Test]
        public void JPEGFromURLTest()
        {
            eyes.Open("Applitools Eyes SDK", "TestJPEGFromURL");
            eyes.Check(Target.Url(new Uri(IMAGE_URL_JPEG)));
            eyes.Close();
        }

        [Test]
        public void PNGFromPathTest()
        {
            var image = new Bitmap(IMAGE_PATH_PNG);
            eyes.Open("Applitools Eyes SDK", "TestPNGFromPath", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(IMAGE_PATH_PNG));
            eyes.Close();
        }

        [Test]
        public void PNGFromBufferTest()
        {
            var image = new Bitmap(IMAGE_PATH_PNG);
            eyes.Open("Applitools Eyes SDK", "TestPNGFromBuffer", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(image));
            eyes.Close();
        }

        [Test]
        public void PNGFromURLTest()
        {
            eyes.Open("Applitools Eyes SDK", "TestPNGFromBuffer");
            eyes.Check(Target.Url(IMAGE_URL_PNG));
            eyes.Close();
        }


        [Test]
        public void IgnoreRegionsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestIgnoreRegionFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).Ignore(new Rectangle(10, 20, 30, 40)));
            eyes.Close();
        }

        [Test]
        public void FloatingRegionsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestFloatingRegionFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).Floating(new Rectangle(10, 20, 30, 40), 5, 10, 20, 15));

            var result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            var floatingRegions = info.ActualAppOutput[0].ImageMatchSettings.Floating;

            TestUtils.compareProcedure(floatingRegions[0], new FloatingMatchSettings(10, 20, 30, 40, 5, 10, 20, 15));
        }

        [Test]
        public void MatchLevelExactFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestMatchLevelExactFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).MatchLevel(MatchLevel.Exact));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Exact, matchLevel);
        }

        [Test]
        public void MatchLevelContentFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestMatchLevelIgnoreColorsFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).MatchLevel(MatchLevel.Content));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Content, matchLevel);
        }
        
        [Test]
        public void MatchLevelIgnoreColorsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestMatchLevelIgnoreColorsFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).MatchLevel(MatchLevel.IgnoreColors));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.IgnoreColors, matchLevel);
        }

        [Test]
        public void MatchLevelStrictFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestMatchLevelStrictFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).MatchLevel(MatchLevel.Strict));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Strict, matchLevel);
        }

        [Test]
        public void MatchLevelLayoutFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestMatchLevelLayoutFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).MatchLevel(MatchLevel.Layout));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Layout2, matchLevel);
        }

        [Test]
        public void MatchLevelExactNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetMatchLevel(MatchLevel.Exact));

            eyes.Open(GetApplicationName(), "TestMatchLevelExactNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Exact, matchLevel);
        }

        [Test]
        public void MatchLevelContentNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetMatchLevel(MatchLevel.Content));

            eyes.Open(GetApplicationName(), "TestMatchLevelContentNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Content, matchLevel);
        }
        
        [Test]
        public void MatchLevelIgnoreColorsNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetMatchLevel(MatchLevel.IgnoreColors));

            eyes.Open(GetApplicationName(), "TestMatchLevelIgnoreColorsNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.IgnoreColors, matchLevel);
        }
        
        [Test]
        public void MatchLevelStrictNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetMatchLevel(MatchLevel.Strict));

            eyes.Open(GetApplicationName(), "TestMatchLevelStrictNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Strict, matchLevel);
        }

        [Test]
        public void MatchLevelLayoutNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetMatchLevel(MatchLevel.Layout));

            eyes.Open(GetApplicationName(), "TestMatchLevelLayoutNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);
            MatchLevel matchLevel = info.ActualAppOutput[0].ImageMatchSettings.MatchLevel;

            Assert.AreEqual(MatchLevel.Layout2, matchLevel);
        }

        //[Test]
        //public void ExtractTextTest()
        //{
        //    eyes.Open(GetApplicationName(), "TestExtractText", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));

        //    var result = eyes.ExtractText(new OcrRegion(EXTRACT_TEXT_IMAGE));
        //    eyes.Close();

        //    Assert.AreEqual(1, result.size());
        //    Assert.AreEqual("This is the navigation bar", result.get(0));
        //}

        [Test]
        public void IgnoreDisplacementsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestIgnoreDisplacementsFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).IgnoreDisplacements());

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            Assert.IsTrue(info.ActualAppOutput[0].ImageMatchSettings.IgnoreDisplacements);
        }

        [Test]
        public void IgnoreDisplacementsNonFluentTest()
        {
            eyes.SetConfiguration(eyes.GetConfiguration().SetIgnoreDisplacements(true));
            eyes.Open(GetApplicationName(), "TestIgnoreDisplacementsNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            Assert.IsTrue(info.ActualAppOutput[0].ImageMatchSettings.IgnoreDisplacements);
        }

        [Test]
        public void CodedRegionsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestCodedRegionsFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE)
                .Ignore(new Rectangle(10, 20, 30, 40))
                .Content(new Rectangle(10, 20, 30, 40))
                .Strict(new Rectangle(10, 20, 30, 40))
                .Layout(new Rectangle(10, 20, 30, 40))
            );

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            Region ignoreRegion = info.ActualAppOutput[0].ImageMatchSettings.Ignore[0];
            Region layoutRegion = info.ActualAppOutput[0].ImageMatchSettings.Layout[0];
            Region contentRegion = info.ActualAppOutput[0].ImageMatchSettings.Content[0];
            Region strictRegion = info.ActualAppOutput[0].ImageMatchSettings.Strict[0];

            TestUtils.compareProcedure(ignoreRegion, new Region(10, 20, 30, 40), "ignore");
            TestUtils.compareProcedure(layoutRegion, new Region(10, 20, 30, 40), "layout");
            TestUtils.compareProcedure(contentRegion, new Region(10, 20, 30, 40), "content");
            TestUtils.compareProcedure(strictRegion, new Region(10, 20, 30, 40), "strict");
        }

        [Test]
        public void EnablePatternsFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestEnablePatternsFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE).EnablePatterns());

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            Assert.IsTrue(info.ActualAppOutput[0].ImageMatchSettings.EnablePatterns);
        }

        [Test]
        public void EnablePatternsNonFluentTest()
        {
            var config = eyes.GetConfiguration();
            config.EnablePatterns = true;
            eyes.SetConfiguration(config);
            eyes.Open(GetApplicationName(), "TestEnablePatternsNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.Check(Target.Image(TEST_IMAGE));

            TestResults result = eyes.Close(false);
            var info = TestUtils.GetSessionResults(eyes.ApiKey, result);

            Assert.IsTrue(info.ActualAppOutput[0].ImageMatchSettings.EnablePatterns);
        }

        //[Test]
        //public void CheckRegionFluentTest()
        //{
        //    eyes.Open(GetApplicationName(), "TestCheckRegionFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
        //    eyes.Check(Target.Region(TEST_IMAGE, new Region(50, 50, 50, 50)));
        //    eyes.Close();
        //}

        [Test]
        public void CheckRegionNonFluentTest()
        {
            eyes.Open(GetApplicationName(), "TestCheckRegionNonFluent", new RectangleSize(TEST_IMAGE.Width, TEST_IMAGE.Height));
            eyes.CheckRegion(TEST_IMAGE, new Rectangle(50, 50, 50, 50));
            eyes.Close();
        }

        [Test]
        public void ExtractTextTest()
        {
	        var image = EXTRACT_TEXT_IMAGE;
	        eyes.Open("Applitools Eyes SDK", "ExtractText", new RectangleSize(image.Width, image.Height));
	        var result = eyes.ExtractText(new OcrRegion(image));
	        Assert.AreEqual(1, result.Count);
	        Assert.AreEqual("This is the navigation bar", result.ElementAt(0));
	        eyes.Close();
        }

        [Test]
        public void ExtractTextRegionsTest()
        {
	        var image = EXTRACT_TEXT_IMAGE_PNG;
	        eyes.Open("Applitools Eyes SDK", "ExtractTextRegions");
	        var result = eyes.ExtractTextRegions(new TextRegionSettings(".+").SetImage(image));
	        Assert.AreEqual(1, result.Count);
            var regions = result.First().Value;
            Assert.AreEqual(1, regions.Count);
            var region = regions.First();
            Assert.AreEqual(new TextRegion(10, 11, 18, 214, "Thisisthenavigationbar"), region);
	        eyes.Close();
        }

        [Test]
        public void LocateTest()
        {
            var image = EXTRACT_TEXT_IMAGE;
            eyes.Open("Eyes Images SDK", "Locate", new RectangleSize(image.Width, image.Height));
            eyes.Check(Target.Image(image));

            var result = eyes.Locate(new VisualLocatorSettings().Name("the").Image(image));
            var regions = result["the"];
            Assert.AreEqual(1, regions.Count);
            var region = regions.First();
            
            Assert.AreEqual(new Region(69, 9, 31, 20), region);
            eyes.Close();
        }
    }
}