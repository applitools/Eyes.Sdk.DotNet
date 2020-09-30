using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using Applitools.Fluent;
using System.Drawing;
using System.Collections.Generic;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Appium.Fluent
{
    internal class SimpleRegionByElement : IGetRegions, IGetAppiumRegion
    {
        private IWebElement element_;

        public SimpleRegionByElement(IWebElement element)
        {
            element_ = element;
        }

        IList<IWebElement> IGetAppiumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }

        public IList<IMutableRegion> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            IWebElement element = element_;
            double scaleRatio = ((Eyes)eyesBase).GetScaleRatioForRegions();
            Region r = new Region(element.Location, element.Size);
            r = r.Scale(scaleRatio);
            eyesBase.Logger.Verbose("screenshot: {0}", screenshot);
            Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
          
            return new MutableRegion[] { new MutableRegion(pTag.X, pTag.Y, r.Width, r.Height) };
        }
    }
}
