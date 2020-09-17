using System.Drawing;
using OpenQA.Selenium;
using Applitools.Appium.Fluent;

namespace Applitools.Appium
{
    public static class Target
    {
        public static AppiumCheckSettings Window()
        {
            return new AppiumCheckSettings();
        }

        public static AppiumCheckSettings Region(Rectangle rect)
        {
            return new AppiumCheckSettings(rect);
        }

        public static AppiumCheckSettings Region(IWebElement element)
        {
            return new AppiumCheckSettings(element);
        }

        public static AppiumCheckSettings Region(By by)
        {
            return new AppiumCheckSettings(by);
        }
    }
}
