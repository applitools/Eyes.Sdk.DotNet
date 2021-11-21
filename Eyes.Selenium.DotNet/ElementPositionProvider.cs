namespace Applitools.Selenium
{
    using Applitools.Selenium.Scrolling;
    using OpenQA.Selenium;
    using System.Drawing;
    using Utils;

    internal class ElementPositionProvider : IPositionProvider, ISeleniumPositionProvider
    {
        private Logger logger_;
        private readonly EyesWebDriver driver_;
        private EyesRemoteWebElement element_;

        public ElementPositionProvider(Logger logger, EyesWebDriver driver, IWebElement element)
        {
            logger_ = logger;
            driver_ = driver;
            if (element is EyesRemoteWebElement eyesElement)
            {
                element_ = eyesElement;
            }
            else
            {
                element_ = new EyesRemoteWebElement(logger, driver, element);
            }
            logger_.Verbose(element.ToString());
        }

        public IWebElement ScrolledElement => element_;

        public Point GetCurrentPosition()
        {
            return element_.ScrollPosition;
        }

        public Size GetEntireSize()
        {
            logger_.Verbose("enter (element_: {0})", element_);
            Size scrollSize = element_.ScrollSize;
            logger_.Verbose(scrollSize.ToString());
            return scrollSize;
        }

        public PositionMemento GetState()
        {
            logger_.Verbose("enter (element_: {0})", element_);
            return new ScrollPositionMemento(GetCurrentPosition());
        }

        public void RestoreState(PositionMemento state)
        {
            ScrollPositionMemento s = (ScrollPositionMemento)state;
            SetPosition(new Point(s.X, s.Y));
        }

        public Point SetPosition(Point location)
        {
            logger_.Verbose("setting position of {0} to {1}", element_, location);
            return element_.ScrollTo(location);
        }
    }
}