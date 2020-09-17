using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class InRegionTest : TestSetup
    {
        public InRegionTest() : base("InRegionTest", new ChromeOptions(), false)
        {
            TestedPageUrl = "https://www.alarm.com/";
            testedPageSize = new Size(1000, 800);
        }


        [Test]
        public void TestInRegion()
        {
            string res = GetEyes().InRegion(By.Id("ctl00_top_navi3_SOnav")).GetText();
            Assert.AreEqual("SOLUTIONS", res);
        }
    }
}
