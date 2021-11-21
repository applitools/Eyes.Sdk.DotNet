using System;
using System.Collections.ObjectModel;

namespace Applitools.OpenQA.Selenium
{
    public interface IWebDriver : IJavaScriptExecutor, IDisposable
    {
        string SessionId { get; }
        string Title { get; }
        string PageSource { get; }
        ReadOnlyCollection<string> WindowHandles { get; }
        string CurrentWindowHandle { get; }
        string Url { get; set; }
        ICapabilities Capabilities { get; }
        bool IsActionExecutor { get; }

        IOptions Manage();
        void Close();
        void Quit();
        ITargetLocator SwitchTo();
        INavigation Navigate();
        void ResetInputState();
        IWebElement FindElement(By by);
        ReadOnlyCollection<IWebElement> FindElements(By by);
    }
}
