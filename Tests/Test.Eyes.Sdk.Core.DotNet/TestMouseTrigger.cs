namespace Applitools
{
    using System;
    using Applitools.Utils.Geometry;
    using NUnit.Framework;

    [TestFixture]
    public class TestMouseTrigger
    {
        [Test]
        public void TestMouseTrigger1()
        {
            Region control = new Region(1, 2, 3, 4);
            Location loc = new Location(5, 6);
            MouseTrigger mt = new MouseTrigger(
                MouseAction.Click,
                control,
                loc);
            Assert.AreEqual(control, mt.Control);
            Assert.AreEqual(loc, mt.Location);
            Assert.AreEqual(MouseAction.Click, mt.MouseAction);
            mt = new MouseTrigger(MouseAction.DoubleClick, Region.Empty, loc);
        }

        [Test]
        public void TestMouseTrigger2()
        {
            Region control = new Region(1, 2, 3, 4);
            Assert.That(
                    () =>
                    {
                        MouseTrigger mt = new MouseTrigger(
                            MouseAction.Click,
                            control,
                            null);
                    },
                    Throws.TypeOf<ArgumentNullException>());
        }
    }
}
