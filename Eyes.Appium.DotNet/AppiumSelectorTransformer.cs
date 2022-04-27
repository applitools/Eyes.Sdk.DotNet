using System;
using System.Linq;
using Applitools.Fluent;
using Applitools.Selenium;
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
                    var parts = by.ToString().Split(':').Select(i => i.Trim()).ToList();
                    return GetSelector(parts[0], parts[1]);
                }
                default:
                    throw new Exception($"Unsupported selector {selector}");
            }
        }

        private RegionSelector GetSelector(string type, string selector)
        {
            switch (type)
            {
                case "By.ClassName[Contains]":
                    return new RegionSelector
                    {
                        Type = "class name",
                        Selector = selector
                    };
                default:
                    return new RegionSelector
                    {
                        Type = type,
                        Selector = selector
                    };
            }
        }
    }
}