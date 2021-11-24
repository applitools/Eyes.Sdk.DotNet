namespace Applitools.Selenium
{
    internal interface IEyesTargetLocator<IWebElement, IWebDriver, FrameChain>
    {
        IWebDriver Frame(IWebElement frame);
        IWebDriver Frames(FrameChain frameChain);
        IWebElement ActiveElement();
    }
}