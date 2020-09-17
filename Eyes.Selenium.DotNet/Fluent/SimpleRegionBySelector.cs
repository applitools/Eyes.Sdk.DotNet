namespace Applitools.Selenium.Fluent
{
    using Utils.Geometry;
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Drawing;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    internal class SimpleRegionBySelector : IGetRegions, IGetSeleniumRegion
    {
        private readonly By selector_;

        public SimpleRegionBySelector(By by)
        {
            selector_ = by;
        }
        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }

        public IList<IMutableRegion> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot)
        {
            EyesWebDriver driver = ((SeleniumEyes)eyesBase).GetDriver();
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            IList<IMutableRegion> mutableRegions = new List<IMutableRegion>();
            foreach (IWebElement element in elements)
            {
                Rectangle r = EyesSeleniumUtils.GetVisibleElementBounds(element);
                Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
                mutableRegions.Add(new MutableRegion(pTag.X, pTag.Y, r.Width, r.Height));
            }
            return mutableRegions;
        }
    }
}
