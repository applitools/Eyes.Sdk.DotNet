using Applitools.Fluent;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
    public class OcrRegion
    {
        private readonly Region _region;
        private readonly IWebElement _element;
        private readonly By _selector;
        private string _hint;
        private string _language;
        private int? _minMatch;
        public TRegion Region { get; set; }

        public OcrRegion(By selector)
        {
            _selector = selector;
        }

        public OcrRegion(IWebElement element)
        {
            _element = element;
        }

        public OcrRegion(Region region)
        {
            _region = region;
        }

        public OcrRegion Hint(string hint)
        {
            _hint = hint;

            return this;
        }

        public string GetHint()
        {
            return _hint;
        }

        public OcrRegion Language(string language)
        {
            _language = language;

            return this;
        }

        public string GetLanguage()
        {
            return _language;
        }

        public OcrRegion MinMatch(int? minMatch)
        {
            _minMatch = minMatch;

            return this;
        }

        public int? GetMinMatch()
        {
            return _minMatch;
        }

        internal By GetSelector()
        {
            return _selector;
        }

        internal IWebElement GetWebElement()
        {
            return _element;
        }

        internal Region GetRegion()
        {
            return _region;
        }
    }
}