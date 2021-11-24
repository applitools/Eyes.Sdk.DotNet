using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Selenium.Capture
{
    class DomCapture : DomCaptureBase
    {
        private readonly EyesWebDriver webDriver_;

        public DomCapture(Logger logger, EyesWebDriver webDriver, UserAgent userAgent) : base(logger, webDriver, userAgent)
        {
            webDriver_ = webDriver;
        }

        protected override void SwitchToDefaultContent()
        {
            webDriver_.SwitchTo().DefaultContent();
        }

        protected override void SwitchToFrame(string xpath)
        {
            IWebElement frame = webDriver_.FindElement(By.XPath(xpath));
            webDriver_.SwitchTo().Frame(frame);
        }

        protected override void SwitchToFrameChain(object fc)
        {
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)webDriver_.SwitchTo();
            switchTo.Frames((FrameChain)fc);
        }

        protected override object GetCurrentFrameChain()
        {
            FrameChain fc = webDriver_.GetFrameChain().Clone();
            logger_.Verbose("saving current frame chain - size: {0} ; frame: {1}", fc.Count, fc.Peek());
            return fc;
        }
    }
}
