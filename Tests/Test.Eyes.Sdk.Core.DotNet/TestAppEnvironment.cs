namespace Applitools
{
    using Applitools.Utils.Geometry;
    using NUnit.Framework;

    [TestFixture]
    public class TestAppEnvironment
    {
        [Test]
        public void TestCtor()
        {
            AppEnvironment ae = new AppEnvironment();
            Assert.IsNull(ae.DisplaySize);
            Assert.IsNull(ae.HostingApp);
            Assert.IsNull(ae.OS);
            ae.DisplaySize = new RectangleSize(10, 100);
            Assert.True(RectangleSize.AreEqual(new RectangleSize(10, 100), ae.DisplaySize));
            ae.HostingApp = "Testing app";
            Assert.AreEqual("Testing app", ae.HostingApp);
            ae.OS = "Some OS";
            Assert.AreEqual("Some OS", ae.OS);
            ae = new AppEnvironment("p1", "p2", new RectangleSize(1, 1));
            Assert.AreEqual("p1", ae.OS);
            Assert.AreEqual("p2", ae.HostingApp);
            Assert.True(RectangleSize.AreEqual(new RectangleSize(1, 1), ae.DisplaySize));
        }

        [Test]
        public void TestToString()
        {
            AppEnvironment ae = new AppEnvironment("p1", "p2", new RectangleSize(1, 1));
            Assert.AreEqual("[OS = 'p1' HostingApp = 'p2' DisplaySize = 1x1]", ae.ToString());
            ae = new AppEnvironment(null, null, new RectangleSize(2, 2));
            Assert.AreEqual("[OS = ? HostingApp = ? DisplaySize = 2x2]", ae.ToString());
        }
    }
}
