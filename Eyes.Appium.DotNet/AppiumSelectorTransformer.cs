using System;
using System.Linq;
using Applitools.Fluent;
using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;

namespace Applitools.Appium
{
    public class AppiumSelectorTransformer : ISelectorTransformer
    {
        public RegionSelector GetRegionSelector(By selector)
        {
            switch (selector)
            {
                case MobileBy mobileBy:
                {
                    var selectorType = mobileBy.GetBaseTypePrivateFieldValue<MobileBy, string>("_searchingCriteriaName");
                    var selectorString = mobileBy.GetPrivateFieldValue<string>("selector");

                    return new RegionSelector
                    {
                        Type = selectorType,
                        Selector = selectorString
                    };
                }
                case By by:
                {
                    var parts = by.ToString()
                        .Split(new[] { ": "}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(i => i.Trim())
                        .ToList();
                    return GetSelector(parts[0], parts[1]);
                }
                default:
                    throw new Exception($"Unsupported selector {selector}");
            }
        }

        private RegionSelector GetSelector(string type, string selector)
        {
            string actualType = string.Empty;
            switch (type)
            {
                case "By.ClassName[Contains]":
                    actualType = "class name";
                    break;
                case "By.TagName":
                    actualType = "tag name";
                    break;
                case "By.Name":
                    actualType = "name";
                    break;
                case "By.Id":
                    actualType = "id";
                    break;
                case "By.CssSelector":
                    actualType = "css selector";
                    break;
                case "By.PartialLinkText":
                    actualType = "partial link text";
                    break;
                case "By.XPath":
                    actualType = "xpath";
                    break;
                case "By.LinkText":
                    actualType = "link text";
                    break;
                default:
                    throw new EyesException($"Unsupported selector {type}");
            }

            return new RegionSelector
            {
                Type = actualType,
                Selector = selector
            };
        }
    }
}