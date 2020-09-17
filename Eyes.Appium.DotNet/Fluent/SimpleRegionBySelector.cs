using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using Applitools.Fluent;
using System.Drawing;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium.Fluent
{
    internal class SimpleRegionBySelector : IGetRegions, IGetAppiumRegion
    {
        private readonly By selector_;

        public SimpleRegionBySelector(By by)
        {
            selector_ = by;
        }
        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            return elements;
        }

        public IList<IMutableRegion> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot)
        {
            IWebDriver driver = ((Eyes)eyesBase).GetDriver();
            ReadOnlyCollection<IWebElement> elements = driver.FindElements(selector_);
            IList<IMutableRegion> mutableRegions = new List<IMutableRegion>();
            double scaleRatio = ((Eyes)eyesBase).GetScaleRatioForRegions();
            foreach (IWebElement element in elements)
            {
                Region r = new Region(element.Location, element.Size);
                r = r.Scale(scaleRatio);
                Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
                mutableRegions.Add(new MutableRegion(pTag.X, pTag.Y, r.Width, r.Height));
            }
            return mutableRegions;
        }
    }
}
