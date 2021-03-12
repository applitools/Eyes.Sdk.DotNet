using Applitools.Utils;
using OpenQA.Selenium;
using System.Drawing;
using Applitools.Selenium.Scrolling;
using Applitools.Utils.Geometry;
using ScrollPositionProvider = Applitools.Selenium.Scrolling.ScrollPositionProvider;

namespace Applitools.Selenium
{
    public class Frame
    {
        private readonly PositionMemento positionMemento_;
        private IEyesJsExecutor jsExecutor_;
        private string originalOverflow_;
        private Logger logger_;

        public Frame(Logger logger, IWebElement reference, Point location, Size outerSize, Size innerSize,
            Point originalLocation, Rectangle bounds, RectangularMargins borderWidths, IEyesJsExecutor jsExecutor)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(reference, nameof(reference));
            ArgumentGuard.NotNull(jsExecutor, nameof(jsExecutor));

            logger.Verbose("Frame(logger, {0}, {1}, {2}, {3}, {4})",
                reference, location, outerSize, innerSize, originalLocation);

            logger_ = logger;
            Reference = reference;
            Location = location;
            OuterSize = outerSize;
            InnerSize = innerSize;
            OriginalLocation = originalLocation;
            Bounds = bounds;
            BorderWidths = borderWidths;
            positionMemento_ = new ScrollPositionMemento(originalLocation);
            jsExecutor_ = jsExecutor;
        }

        public IWebElement Reference { get; private set; }

        public Point Location { get; private set; }

        public Size OuterSize { get; private set; }

        public Size InnerSize { get; private set; }

        public Point OriginalLocation { get; private set; }

        internal IWebElement ScrollRootElement { get; set; }

        public Rectangle Bounds { get; private set; }

        public RectangularMargins BorderWidths { get; private set; }
        public Rectangle ScrollRootElementInnerBounds { get; internal set; }

        public void ReturnToOriginalPosition(IWebDriver driver)
        {
            IWebElement scrollRootElement = GetScrollRootElement(driver);
            IPositionProvider positionProvider = new ScrollPositionProvider(logger_, jsExecutor_, scrollRootElement);
            positionProvider.RestoreState(positionMemento_);
        }

        public void HideScrollbars(IWebDriver driver)
        {
            IWebElement scrollRootElement = GetScrollRootElement(driver);
            logger_.Verbose("hiding scrollbars of element: {0}", scrollRootElement);
            originalOverflow_ = (string)jsExecutor_.ExecuteScript("var origOF = arguments[0].style.overflow; arguments[0].style.overflow='hidden'; return origOF;", scrollRootElement);
        }

        public void ReturnToOriginalOverflow(IWebDriver driver)
        {
            IWebElement scrollRootElement = GetScrollRootElement(driver);
            logger_.Verbose("returning overflow of element to its original value: {0}", scrollRootElement);
            jsExecutor_.ExecuteScript($"arguments[0].style.overflow='{originalOverflow_}';", scrollRootElement);
        }

        private IWebElement GetScrollRootElement(IWebDriver driver)
        {
            IWebElement scrollRootElement = ScrollRootElement;
            if (scrollRootElement == null)
            {
                logger_.Verbose("no scroll root element. selecting default.");
                scrollRootElement = EyesSeleniumUtils.GetDefaultRootElement(driver);
            }

            return scrollRootElement;
        }

        public override string ToString()
        {
            return Reference?.ToString();
        }
    }
}