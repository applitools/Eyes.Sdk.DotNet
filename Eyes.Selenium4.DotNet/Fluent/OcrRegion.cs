using Applitools.Fluent;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
    public class OcrRegion
    {
        private readonly Region region_;
        private readonly IWebElement element_;
        private readonly By selector_;
        private string hint_;
        private string language_;
        private int? minMatch_;
        public TRegion Region { get; set; }

        public OcrRegion(By selector)
        {
            selector_ = selector;
        }

        public OcrRegion(IWebElement element)
        {
            element_ = element;
        }

        public OcrRegion(Region region)
        {
            region_ = region;
        }

        public OcrRegion Hint(string hint)
        {
            hint_ = hint;

            return this;
        }

        public string GetHint()
        {
            return hint_;
        }

        public OcrRegion Language(string language)
        {
            language_ = language;

            return this;
        }

        public string GetLanguage()
        {
            return language_;
        }

        public OcrRegion MinMatch(int? minMatch)
        {
            minMatch_ = minMatch;

            return this;
        }

        public int? GetMinMatch()
        {
            return minMatch_;
        }

        internal By GetSelector()
        {
            return selector_;
        }

        internal IWebElement GetWebElement()
        {
            return element_;
        }

        internal Region GetRegion()
        {
            return region_;
        }
    }
}