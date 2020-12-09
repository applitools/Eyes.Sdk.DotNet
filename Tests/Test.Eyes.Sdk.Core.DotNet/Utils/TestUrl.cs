using System;
using Applitools.Tests.Utils;
using NUnit.Framework;

namespace Applitools.Utils
{
    [TestFixture]
    public class TestUrl : ReportingTestSuite
    {
        private const string Url_ = "http://www.google.com";

        [Test]
        public void TestUrlString()
        {
            Url url = new Url(Url_);
            Assert.AreEqual(url.AbsolutePath, "/");
            Assert.AreEqual(Url_ + "/", url.AbsoluteUri);
            Assert.That(() => { url = new Url(string.Empty); }, Throws.TypeOf<UriFormatException>());
        }

        [Test]
        public void TestUrlUri()
        {
            Url url = new Url(new Uri(Url_));
            Assert.AreEqual(url.AbsolutePath, "/");
            Assert.AreEqual(Url_ + "/", url.AbsoluteUri);
        }

        [Test]
        public void TestQueryElement1()
        {
            Url url = new Url(Url_).QueryElement("search", "Unit Testing");
            Assert.AreEqual(url.Query, "?search=Unit%20Testing");
            Assert.That(() =>
            {
                url = url.QueryElement(string.Empty, "someValue");
            }, Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void TestQueryElement2()
        {
            Url url = new Url(Url_)
                .QueryElement("search", "Unit Testing")
                .QueryElement("search1", "Best practice");
            Assert.AreEqual(url.Query, "?search=Unit%20Testing&search1=Best%20practice");
            Assert.That(() =>
            {
                url = url.QueryElement("parameter", null);
            }, Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        public void TestSubpathElement()
        {
            string expected = Url_;
            Url url = new Url(Url_).SubpathElement("search");
            expected += "/search";
            Assert.AreEqual(expected, url.AbsoluteUri);
            url = url.SubpathElement("/search1");
            expected += "/search1";
            Assert.AreEqual(expected, url.AbsoluteUri);
            url = url.SubpathElement("v1/api/runningsessions");
            expected += "/v1/api/runningsessions";
            Assert.AreEqual(expected, url.AbsoluteUri);
            Assert.That(() =>
            {
                url.SubpathElement(string.Empty);
            }, Throws.TypeOf<ArgumentException>());
        }
    }
}
