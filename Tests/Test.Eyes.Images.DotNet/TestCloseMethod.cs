using Applitools.Exceptions;
using Applitools.Tests.Utils;
using NUnit.Framework;
using System.Drawing;
using System.IO;

namespace Applitools.Images.Tests
{
    [TestFixture]
    [Parallelizable]
    public class TestCloseMethod : ReportingTestSuite
    {
        [Test]
        public void TestClose()
        {
            BatchInfo batch = new BatchInfo();
            Eyes eyes = new Eyes();
            eyes.Batch = batch;

            eyes.Open(nameof(TestCloseMethod), nameof(TestClose));
            eyes.CheckImage(new Bitmap(TestEyesImages.ReadResourceStream_("gbg1.png")), "TestBitmap1");
            eyes.Close(false);

            eyes.Open(nameof(TestCloseMethod), nameof(TestClose));
            eyes.CheckImage(new Bitmap(TestEyesImages.ReadResourceStream_("twitter1d-s4.png")), "TestBitmap1");
            Assert.That(() => eyes.Close(true), Throws.InstanceOf<DiffsFoundException>());
        }

        private static string Path_(string name)
        {
            return Path.GetFullPath(Path.Combine("Content", name));
        }

    }
}
