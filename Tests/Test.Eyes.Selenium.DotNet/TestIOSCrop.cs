using Applitools.Selenium.Capture;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Applitools.Selenium.Tests
{
    public class TestIOSCrop : ReportingTestSuite
    {

        public static IEnumerable IOSDeviceFixtureArgs
        {
            get
            {
                #region iPads, Landscape

                // resolution: 2160 x 1620 ; viewport: 2160 x 1480
                // [iPad (8th generation), 14.4]
                yield return new TestCaseData("iPad (8th generation) Simulator", "14.4", ScreenOrientation.Landscape, 2160, 1480);

                // resolution: 2048 x 1536 ; viewport: 2048 x 1408 
                // [iPad Air 2, 10.3]
                yield return new TestCaseData("iPad Air 2 Simulator", "10.3", ScreenOrientation.Landscape, 2048, 1408);

                // resolution: 2048 x 1536 ; viewport: 2048 x 1396
                // [iPad Air 2, 12.0] [iPad Air 2, 11.0] [iPad Air, 12.0] [iPad Air, 11.0] [iPad Pro (9.7 inch), 11.0] [iPad, 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "12.0", ScreenOrientation.Landscape, 2048, 1396);

                // resolution: 2048 x 1536 ; viewport: 2048 x 1331
                // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "11.3", ScreenOrientation.Landscape, 2048, 1331);

                // resolution: 2732 x 2048 ; viewport: 2732 x 1908
                // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
                yield return new TestCaseData("iPad Pro (12.9 inch) (2nd generation) Simulator", "11.0", ScreenOrientation.Landscape, 2732, 1908);

                // resolution: 2224 x 1668 ; viewport: 2224 x 1528
                // [iPad Pro (10.5 inch), 11.0]
                yield return new TestCaseData("iPad Pro (10.5 inch) Simulator", "11.0", ScreenOrientation.Landscape, 2224, 1528);
                #endregion

                #region iPads, Portrait

                // resolution: 1536 x 2048; viewport: 1536 x 1843
                // [iPad Air 2, 11.3] [iPad (5th generation), 11.0]
                yield return new TestCaseData("iPad (5th generation) Simulator", "11.0", ScreenOrientation.Portrait, 1536, 1843);
              
                // resolution: 1620 x 2160 ; viewport: 1620 x 2020
                // [iPad (8th generation), 14.4]
                yield return new TestCaseData("iPad (8th generation) Simulator", "14.4", ScreenOrientation.Portrait, 1620, 2020);

                // resolution: 1536 x 2048; viewport: 1536 x 1920 
                // [iPad Air 2, 10.3]
                yield return new TestCaseData("iPad Air 2 Simulator", "10.3", ScreenOrientation.Portrait, 1536, 1920);

                // resolution: 1536 x 2048; viewport: 1536 x 1908 
                // [iPad Air 2, 11.0] [iPad Air 2, 12.0] [iPad Air, 11.0] [iPad, 11.0] [iPad Pro (9.7 inch), 12.0] [iPad Pro (9.7 inch), 11.0]
                yield return new TestCaseData("iPad Air 2 Simulator", "11.0", ScreenOrientation.Portrait, 1536, 1908);

                // resolution: 2048 x 2732 ; viewport: 2048 x 2592
                // [iPad Pro (12.9 inch) (2nd generation), 11.0] [iPad Pro (12.9 inch) (2nd generation), 12.0]
                yield return new TestCaseData("iPad Pro (12.9 inch) (2nd generation) Simulator", "11.0", ScreenOrientation.Portrait, 2048, 2592);
               
                // resolution: 2048 x 2732 ; viewport: 2048 x 2542
                // [iPad Pro (12.9 inch) (5th generation), 14.0]
                yield return new TestCaseData("iPad Pro (12.9 inch) (5th generation) Simulator", "14.0", ScreenOrientation.Portrait, 2048, 2542);

                // resolution: 1668 x 2224 ; viewport: 1668 x 2084
                // [iPad Pro (10.5 inch), 11.0]
                yield return new TestCaseData("iPad Pro (10.5 inch) Simulator", "11.0", ScreenOrientation.Portrait, 1668, 2084);

                #endregion

                #region iPhones, Landscape

                // resolution: 2436 x 1125 ; viewport: 2172 x 912
                // [iPhone XS, 12.2] [iPhone X, 11.2]
                yield return new TestCaseData("iPhone XS Simulator", "12.2", ScreenOrientation.Landscape, 2172, 912);

                // resolution: 2436 x 1125 ; viewport: 2172 x 813
                // [iPhone 11 Pro, 13.0]
                yield return new TestCaseData("iPhone 11 Pro Simulator", "13.0", ScreenOrientation.Landscape, 2172, 813);

                // resolution: 2688 x 1242 ; viewport: 2424 x 1030
                // [iPhone XS Max, 12.2]
                yield return new TestCaseData("iPhone XS Max Simulator", "12.2", ScreenOrientation.Landscape, 2424, 1030);

                // resolution: 2688 x 1242 ; viewport: 2424 x 922
                // [iPhone 11 Pro Max, 13.0]
                yield return new TestCaseData("iPhone 11 Pro Max Simulator", "13.0", ScreenOrientation.Landscape, 2424, 922);

                // resolution: 1792 x 828 ; viewport: 1616 x 686
                // [iPhone XR, 12.2]
                yield return new TestCaseData("iPhone XR Simulator", "12.2", ScreenOrientation.Landscape, 1616, 686);

                // resolution: 1792 x 828 ; viewport: 1616 x 620
                // [iPhone 11, 13.0]
                yield return new TestCaseData("iPhone 11 Simulator", "13.0", ScreenOrientation.Landscape, 1616, 620);

                // resolution: 2208 x 1242 ; viewport: 2208 x 1092
                // [iPhone 6 Plus, 11.0]
                yield return new TestCaseData("iPhone 6 Plus Simulator", "11.0", ScreenOrientation.Landscape, 2208, 1092);

                // resolution: 1334 x 750 ; viewport: 1334 x 662
                // [iPhone 7, 10.3]
                yield return new TestCaseData("iPhone 7 Simulator", "10.3", ScreenOrientation.Landscape, 1334, 662);

                // resolution: 2208 x 1242 ; viewport: 2208 x 1110
                // [iPhone 7 Plus, 10.3]
                yield return new TestCaseData("iPhone 7 Plus Simulator", "10.3", ScreenOrientation.Landscape, 2208, 1110);

                // resolution: 1136 x 640 ; viewport: 1136 x 464
                // [iPhone 5s, 10.3]
                yield return new TestCaseData("iPhone 5s Simulator", "10.3", ScreenOrientation.Landscape, 1136, 464);

                #endregion

                #region iPhones, Portrait

                // resolution: 1125 x 2436 ; viewport: 1125 x 1905
                // [iPhone XS, 12.2] [iPhone X, 11.2] [iPhone 11 Pro, 13.0]
                yield return new TestCaseData("iPhone XS Simulator", "12.2", ScreenOrientation.Portrait, 1125, 1905);

                // resolution: 1242 x 2688 ; viewport: 1242 x 2157
                // [iPhone XS Max, 12.2] [iPhone 11 Pro Max, 13.0]
                yield return new TestCaseData("iPhone XS Max Simulator", "12.2", ScreenOrientation.Portrait, 1242, 2157);

                // resolution: 828 x 1792 ; viewport: 828 x 1438
                // [iPhone XR, 12.2] [iPhone 11, 13.0]
                yield return new TestCaseData("iPhone XR Simulator", "12.2", ScreenOrientation.Portrait, 828, 1438);

                // resolution: 1242 x 2208 ; viewport: 1242 x 1866
                // [iPhone 6 Plus, 11.0] [iPhone 7 Plus, 10.3]
                yield return new TestCaseData("iPhone 6 Plus Simulator", "11.0", ScreenOrientation.Portrait, 1242, 1866);

                // resolution: 750 x 1334 ; viewport: 750 x 1118
                // [iPhone 7, 10.3]
                yield return new TestCaseData("iPhone 7 Simulator", "10.3", ScreenOrientation.Portrait, 750, 1118);

                // resolution: 640 x 1136 ; viewport: 640 x 920
                // [iPhone 5s, 10.3]
                yield return new TestCaseData("iPhone 5s Simulator", "10.3", ScreenOrientation.Portrait, 640, 920);

                #endregion
            }
        }

        [Test]
        [TestCaseSource(nameof(IOSDeviceFixtureArgs))]
        public void TestIOSSafariCrop(string deviceName, string platformVersion, ScreenOrientation deviceOrientation, int vpWidth, int vpHeight)
        {
            string testName = GetResourceName_(deviceName, platformVersion, deviceOrientation);
            using (Stream inputStream = CommonUtils.ReadResourceStream("Test.Eyes.Selenium.DotNet.Resources.IOSImages.Input." + testName + ".png"))
            using (Stream expectedStream = CommonUtils.ReadResourceStream("Test.Eyes.Selenium.DotNet.Resources.IOSImages.Expected." + testName + ".png"))
            {
                Assert.NotNull(inputStream, nameof(inputStream));
                Assert.NotNull(expectedStream, nameof(expectedStream));
                if (inputStream != null && expectedStream != null)
                {
                    using (Bitmap input = new Bitmap(inputStream))
                    using (Bitmap expected = new Bitmap(expectedStream))
                    {
                        Assert.NotNull(input, nameof(input));
                        Assert.NotNull(expected, nameof(expected));

                        Bitmap output = SafariScreenshotImageProvider.CropIOSImage(input, new Size(vpWidth, vpHeight));
                        if (!TestUtils.RUNS_ON_CI)
                        {
                            string path = Path.Combine(TestUtils.LOGS_PATH, "DotNet", "IOSCrop");
                            Directory.CreateDirectory(path);
                            input.Save(Path.Combine(path, testName + "_input.png"));
                            output.Save(Path.Combine(path, testName + "_output.png"));
                        }
                        Assert.AreEqual(expected.Width, output.Width, "Width");
                        Assert.AreEqual(expected.Height, output.Height, "Height");
                        Assert.IsTrue(AreBitmapsSame_(output, expected), "Bitmap comparison");
                    }
                }
            }
        }
        private static string GetResourceName_(string deviceName, string platformVersion, ScreenOrientation deviceOrientation)
        {
            return $"{deviceName} {platformVersion} {deviceOrientation}";
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
