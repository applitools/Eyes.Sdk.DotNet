using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable]
    public class DynamicPagesTest : TestSetup
    {
        public DynamicPagesTest() : base(nameof(DynamicPagesTest), new ChromeOptions(), false)
        {
            testedPageSize = new Size(980, 460);
        }

        [Test]
        public void TestNbcNews()
        {
            TestedPageUrl = "https://www.nbcnews.com/";
            GetEyes().Check("NBC News Test", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestEbay()
        {
            TestedPageUrl = "https://www.ebay.com/";
            GetEyes().Check("ebay Test", Target.Window().Fully().SendDom());
            GetDriver().FindElement(By.LinkText("Electronics")).Click();
            GetEyes().Check("ebay Test - Electroincs", Target.Window().Fully().SendDom());
            GetDriver().FindElement(By.LinkText("Smart Home")).Click();
            GetEyes().Check("ebay Test - Electroincs > Smart Home", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestAliExpress()
        {
            TestedPageUrl = "https://www.aliexpress.com/";
            GetEyes().Check("AliExpress Test", Target.Window().Fully().SendDom());
        }

        [Test]
        public void TestBestBuy()
        {
            TestedPageUrl = "https://www.bestbuy.com/site/apple-macbook-pro-13-display-intel-core-i5-8-gb-memory-256gb-flash-storage-silver/6936477.p?skuId=6936477";
            GetDriver().FindElements(By.CssSelector(".us-link"))[0].Click();
            GetEyes().Check("BestBuy Test", Target.Window().Fully().SendDom());
        }
    }
}
