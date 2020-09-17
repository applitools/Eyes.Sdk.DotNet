using Applitools.Tests.Utils;
using Applitools.Utils.Images;
using NUnit.Framework;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace Applitools
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestImageUtils : ReportingTestSuite
    {
        private string testScaleDownImageOutputPath_ = TestUtils.InitLogPath(nameof(TestScaleDownImage));

        [TestCase("1-2")]
        [TestCase("1-3")]
        [TestCase("2-3")]
        [TestCase("1-4")]
        public void TestScaleDownImage(string baseName)
        {
            Bitmap src = LoadBitmapFromResource(baseName + "_orig.png");
            Bitmap expected = LoadBitmapFromResource(baseName + "_scaled.png");
            Bitmap dest = BasicImageUtils.ScaleImage(src, expected.Width / (double)src.Width);
            if (!TestUtils.RUNS_ON_CI)
            {
                Directory.CreateDirectory(testScaleDownImageOutputPath_);
                src.Save(Path.Combine(testScaleDownImageOutputPath_, baseName + "_orig.png"));
                expected.Save(Path.Combine(testScaleDownImageOutputPath_, baseName + "_expected.png"));
                dest.Save(Path.Combine(testScaleDownImageOutputPath_, baseName + "_actual_result.png"));
            }
            CompareBitmaps_(expected, dest);
        }

        [Test]
        public void TestScaleDownImage1()
        {
            Bitmap src = LoadBitmapFromResource("SrcImage.png");
            Bitmap expected = LoadBitmapFromResource("ScaledDownImage.png");
            Bitmap dest = BasicImageUtils.ScaleImage(src, expected.Width / (double)src.Width);
            if (!TestUtils.RUNS_ON_CI)
            {
                Directory.CreateDirectory(testScaleDownImageOutputPath_);
                src.Save(Path.Combine(testScaleDownImageOutputPath_, "orig.png"));
                expected.Save(Path.Combine(testScaleDownImageOutputPath_, "expected.png"));
                dest.Save(Path.Combine(testScaleDownImageOutputPath_, "actual_result.png"));
            }
            CompareBitmaps_(expected, dest);
        }

        [Test]
        public void TestScaleDownImage2()
        {
            Bitmap src = LoadBitmapFromResource("scale_bug_to_540x768.png");
            Bitmap expected = LoadBitmapFromResource("ScaledDownTo540x768Image.png");
            Bitmap dest = BasicImageUtils.ScaleImage(src, 540 / (double)src.Width);
            CompareBitmaps_(expected, dest);
        }

        private static Bitmap LoadBitmapFromResource(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            using (Stream stream = thisAssembly.GetManifestResourceStream("Test.Eyes.Sdk.Core.DotNet.Resources." + filename))
            {
                Bitmap bitmap = (Bitmap)Image.FromStream(stream);
                return bitmap;
            }
        }

        private static string Path_(string srcName)
        {
            return Path.GetFullPath(Path.Combine("Resources", srcName));
        }

        private static void CompareBitmaps_(Bitmap expected, Bitmap dest)
        {
            Assert.AreEqual(expected.Width, dest.Width, "Widths differ");
            Assert.AreEqual(expected.Height, dest.Height, "Heights differ");
            Assert.AreEqual(expected.PixelFormat, dest.PixelFormat, "Pixel formats differ");
            bool areSame = AreBitmapsSame_(expected, dest);
            Assert.IsTrue(areSame, "Images differ");
        }

        private static bool AreBitmapsSame_(Bitmap bmp1, Bitmap bmp2)
        {
            bool equals = true;
            Rectangle rect = new Rectangle(0, 0, bmp1.Width, bmp1.Height);
            BitmapData bmpData1 = bmp1.LockBits(rect, ImageLockMode.ReadOnly, bmp1.PixelFormat);
            BitmapData bmpData2 = bmp2.LockBits(rect, ImageLockMode.ReadOnly, bmp2.PixelFormat);
            unsafe
            {
                byte* ptr1 = (byte*)bmpData1.Scan0.ToPointer();
                byte* ptr2 = (byte*)bmpData2.Scan0.ToPointer();
                int width = rect.Width * Image.GetPixelFormatSize(bmp1.PixelFormat) >> 3;
                for (int y = 0; equals && y < rect.Height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (*ptr1 != *ptr2)
                        {
                            equals = false;
                            break;
                        }
                        ptr1++;
                        ptr2++;
                    }
                    ptr1 += bmpData1.Stride - width;
                    ptr2 += bmpData2.Stride - width;
                }
            }
            bmp1.UnlockBits(bmpData1);
            bmp2.UnlockBits(bmpData2);

            return equals;
        }
    }
}
