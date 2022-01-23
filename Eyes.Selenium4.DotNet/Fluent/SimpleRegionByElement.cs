using Applitools.Utils;

namespace Applitools.Selenium.Fluent
{
    using Utils.Geometry;
    using OpenQA.Selenium;
    using Applitools.Fluent;
    using System.Drawing;
    using System.Collections.Generic;

    internal class SimpleRegionByElement : IGetRegions, IGetSeleniumRegion
    {
        private IWebElement element_;

        public SimpleRegionByElement(IWebElement element)
        {
            element_ = element;
        }

        IList<IWebElement> IGetSeleniumRegion.GetElements(IWebDriver driver)
        {
            return new List<IWebElement>() { element_ };
        }

        public IList<IMutableRegion> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            if (!(element_ is EyesWebElement eyesElement))
            {
                eyesElement = new EyesWebElement(eyesBase.Logger, ((SeleniumEyes)eyesBase).GetDriver(), element_);
            }
            Rectangle r = EyesSeleniumUtils.GetVisibleElementBounds(eyesElement);
            Point pTag = screenshot.ConvertLocation(r.Location, CoordinatesTypeEnum.CONTEXT_RELATIVE, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
            return new MutableRegion[] { new MutableRegion(pTag.X, pTag.Y, r.Width, r.Height) };
        }

        public TRegion ToRegion()
        {
            var elementId = element_.GetPrivateFieldValue<string>("elementId");

            return new ElementRegion
            {
                ElementId = elementId
            };
        }
    }
}
