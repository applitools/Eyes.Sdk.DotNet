using OpenQA.Selenium;
using EyesBy = Applitools.OpenQA.Selenium.By;
using Applitools.Utils;

namespace Applitools.Selenium
{
    static class Utils
    {

        internal static By ToBy(this EyesBy eyesBy)
        {
            ArgumentGuard.NotNull(eyesBy, "By");
            switch (eyesBy.Mechanism)
            {
                case "Id": return By.Id(eyesBy.Criteria);
                case "LinkText": return By.LinkText(eyesBy.Criteria);
                case "Name": return By.Name(eyesBy.Criteria);
                case "XPath": return By.XPath(eyesBy.Criteria);
                case "ClassName": return By.ClassName(eyesBy.Criteria);
                case "PartialLinkText": return By.PartialLinkText(eyesBy.Criteria);
                case "TagName": return By.TagName(eyesBy.Criteria);
                case "CssSelector": return By.CssSelector(eyesBy.Criteria);
            }
            return null;
        }

    }
}