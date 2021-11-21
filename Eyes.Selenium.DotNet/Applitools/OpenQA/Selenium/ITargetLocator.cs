namespace Applitools.OpenQA.Selenium
{
    public interface ITargetLocator
    {
        IWebDriver DefaultContent();
        IAlert Alert();
        IWebElement ActiveElement();
        IWebDriver Frame(IWebElement targetFrame);
        IWebDriver ParentFrame();
    }
}