using OpenQA.Selenium;

namespace Applitools.Selenium.Fluent
{
    internal class FrameLocator : ISeleniumFrameCheckTarget
    {
        public int? FrameIndex { get; set; }
        public string FrameNameOrId { get; set; }
        public IWebElement FrameReference { get; set; }
        public By FrameSelector { get; set; }
        public IWebElement ScrollRootElement { get; set; }
        public By ScrollRootSelector { get; set; }

        public int? GetFrameIndex()
        {
            return FrameIndex;
        }

        public string GetFrameNameOrId()
        {
            return FrameNameOrId;
        }

        public IWebElement GetFrameReference()
        {
            return FrameReference;
        }

        public By GetFrameSelector()
        {
            return FrameSelector;
        }

        public IWebElement GetScrollRootElement()
        {
            return ScrollRootElement;
        }

        public By GetScrollRootSelector()
        {
            return ScrollRootSelector;
        }

        public override string ToString()
        {
            string locator = "";
            if (FrameIndex.HasValue) locator = $"index: {FrameIndex.Value}";
            else if (FrameNameOrId != null) locator = $"name or id: '{FrameNameOrId}'";
            else if (FrameSelector != null) locator = $"selector: {FrameSelector}";
            else if (FrameReference != null) locator = $"reference: {FrameReference}";

            return $"{nameof(FrameLocator)}: {locator}";
        }
    }
}