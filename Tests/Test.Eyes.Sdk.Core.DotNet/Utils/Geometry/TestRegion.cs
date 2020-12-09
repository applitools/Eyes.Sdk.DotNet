using System;
using System.Collections.Generic;
using System.Drawing;
using Applitools.Utils.Geometry;
using NUnit.Framework;

using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Tests.Utils.Geometry
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestRegion : ReportingTestSuite
    {
        [Test]
        public void TestCtor()
        {
            Region r = new Region(1, 2, 3, 4);
            Assert.AreEqual(1, r.Left);
            Assert.AreEqual(2, r.Top);
            Assert.AreEqual(3, r.Width);
            Assert.AreEqual(4, r.Height);
            Assert.That(() => { new Region(1, 1, -1, 1); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void TestToString()
        {
            Region r = new Region(0, 0, 1, 1);
            Assert.AreEqual("(0, 0) 1x1", r.ToString());
        }

        [Test]
        public void TestRegionSplitting_iPhone_XS_12_2_Portrait()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 375, 2255);
            ICollection<SubregionForStitching> regions = region.GetSubRegions(new Size(375, 635), 10, 3, new Rectangle(0, 0, 1125, 1905), logger);
            ICollection<SubregionForStitching> expectedRegions = new SubregionForStitching[] {
                new SubregionForStitching(new Point(0, 0), new Point(0, 0), new Rectangle(0, 0, 1125, 1905), new Rectangle(0, 0, 375, 635)),
                new SubregionForStitching(new Point(0, 615), new Point(0, 625), new Rectangle(0, 0, 1125, 1905), new Rectangle(0, 10, 375, 625)),
                new SubregionForStitching(new Point(0, 1230), new Point(0, 1240), new Rectangle(0, 0, 1125, 1905), new Rectangle(0, 10, 375, 625)),
                new SubregionForStitching(new Point(0, 1620), new Point(0, 1825), new Rectangle(0, 585, 1125, 1320), new Rectangle(0, 10, 375, 430))
            };
            CollectionAssert.AreEqual(expectedRegions, regions);
            // ScrollTo: { X = 0,Y = 0}; PasteLocation: { X = 0,Y = 0}; PhysicalCropArea: { X = 0,Y = 0,Width = 1125,Height = 1905}; LogicalCropArea { X = 0,Y = 0,Width = 375,Height = 635}
            // ScrollTo: { X = 0,Y = 615}; PasteLocation: { X = 0,Y = 625}; PhysicalCropArea: { X = 0,Y = 0,Width = 1125,Height = 1905}; LogicalCropArea { X = 0,Y = 10,Width = 375,Height = 625}
            // ScrollTo: { X = 0,Y = 1230}; PasteLocation: { X = 0,Y = 1240}; PhysicalCropArea: { X = 0,Y = 0,Width = 1125,Height = 1905}; LogicalCropArea { X = 0,Y = 10,Width = 375,Height = 625}
            // ScrollTo: { X = 0,Y = 1620}; PasteLocation: { X = 0,Y = 1827}; PhysicalCropArea: { X = 0,Y = 585,Width = 1125,Height = 1320}; LogicalCropArea { X = 0,Y = 10,Width = 375,Height = 430}
        }

        [Test]
        public void TestRegionSplitting_NegativeHeight()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 375, 2255);
            Assert.That(() => region.GetSubRegions(new Size(375, 635), 10, 3, new Rectangle(0, 0, 1125, -100), logger),
                Throws.InstanceOf<ArgumentOutOfRangeException>());

        }

        //[Test]
        public void TestRegionSplitting_4()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 900, 1194);
            ICollection<SubregionForStitching> regions = region.GetSubRegions(new Size(386, 512), 10, 4, new Rectangle(0, 0, 1544, 2048), logger);
        }

        //[Test]
        public void TestRegionSplitting_3()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 900, 1194);
            ICollection<SubregionForStitching> regions = region.GetSubRegions(new Size(386, 512), 10, 3, new Rectangle(0, 0, 1158, 1536), logger);
        }

        //[Test]
        public void TestRegionSplitting_2()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 900, 1194);
            ICollection<SubregionForStitching> regions = region.GetSubRegions(new Size(386, 512), 10, 2, new Rectangle(0, 0, 772, 1024), logger);
        }

        //[Test]
        public void TestRegionSplitting_1()
        {
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            Region region = new Region(0, 0, 900, 1194);
            ICollection<SubregionForStitching> regions = region.GetSubRegions(new Size(386, 512), 10, 1, new Rectangle(0, 0, 386, 512), logger);
        }
    }
}
