using System.Linq;
using Applitools.Fluent;
using OpenQA.Selenium;

namespace Applitools.Selenium
{
    public class SeleniumSelectorTransformer : ISelectorTransformer
    {
        public RegionSelector GetRegionSelector(By selector)
        {
            var parts = selector.ToString().Split(new[] { ':' }, 2).Select(i => i.Trim()).ToList();

            return GetSelector(parts[0], parts[1]);
        }

        private RegionSelector GetSelector(string type, string selector)
        {
            string cssSelectorName = "css selector";
            switch (type)
            {
                case "By.ClassName":
                    return new RegionSelector
                    {
                        Type = cssSelectorName,
                        Selector = $".{selector}"
                    };
                case "By.TagName":
                    return new RegionSelector
                    {
                        Type = cssSelectorName,
                        Selector = selector
                    };
                case "By.Name":
                    return new RegionSelector
                    {
                        Type = cssSelectorName,
                        Selector = $"[name=\"{selector}\"]"
                    };
                case "By.Id":
                    return new RegionSelector
                    {
                        Type = cssSelectorName,
                        Selector = $"[id=\"{selector}\"]"
                    };
                case "By.CssSelector":
                    return new RegionSelector
                    {
                        Type = cssSelectorName,
                        Selector = selector
                    };
                case "By.PartialLinkText":
                    return new RegionSelector
                    {
                        Type = "partial link text",
                        Selector = selector
                    };
                case "By.XPath":
                    return new RegionSelector
                    {
                        Type = "xpath",
                        Selector = selector
                    };
                case "By.LinkText":
                    return new RegionSelector
                    {
                        Type = "link text",
                        Selector = selector
                    };
                default:
                    throw new EyesException($"Unsupported selector {type}");
            }
        }
    }
}