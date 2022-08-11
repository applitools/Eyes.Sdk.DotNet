using Applitools.Selenium;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Eyes.Selenium.UnitTests
{
    public class SeleniumSelectorTransformerTests
    {
        private SeleniumSelectorTransformer _seleniumSelectorTransformer;

        [SetUp]
        public void Setup()
        {
            _seleniumSelectorTransformer = new SeleniumSelectorTransformer();
        }

        [Test]
        public void XpathSelector_MultipleColons_RegionSelectorCreated()
        {
            string xpath = "//div[text()='Eye Pressures']/ancestor::div[contains(@class, 'dynamic-exam_examContainer')]";
            var selector =  _seleniumSelectorTransformer.GetRegionSelector(By.XPath(xpath));
            Assert.AreEqual(xpath, selector.Selector);
        }
    }
}