namespace Applitools.OpenQA.Selenium
{
    public interface ITargetLocator
    {
        IWebDriver DefaultContent();
        IAlert Alert();
        IWebElement ActiveElement();
        IWebDriver Frame(IWebElement targetFrame);
        IWebDriver Frame(int frameIndex);
        IWebDriver Frame(string frameNameOrId);
        IWebDriver ParentFrame();
        IWebDriver Window(string windowName);
    }
}