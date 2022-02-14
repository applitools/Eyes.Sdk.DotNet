using System.Linq;
using Applitools.Fluent;

namespace Applitools.Selenium
{
    public class SelectorTransformer
    {
        public RegionSelector GetRegionSelector(string description)
        {
            var parts = description.Split(':').Select(i => i.Trim()).ToList();

            return new RegionSelector
            {
                Type = GetSelector(parts[0]),
                Selector = parts[1]
            };
        }

        private string GetSelector(string part)
        {
            switch (part)
            {
                case "By.ClassName":
                    return "class name";
                case "By.PartialLinkText":
                    return "partial link text";
                case "By.TagName":
                    return "tag name";
                case "By.Name":
                    return "name";
                case "By.CssSelector":
                    return "css selector";
                case "By.XPath":
                    return "xpath";
                case "By.LinkText":
                    return "link text";
                case "By.Id":
                    return "id";
                default:
                    throw new EyesException($"Unsupported selector {part}");
            }
        }
    }
}