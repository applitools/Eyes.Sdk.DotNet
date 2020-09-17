using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Selenium.Scrolling
{
    internal class EdgeBrowserScrollPositionProvider : ScrollPositionProvider
    {
        public EdgeBrowserScrollPositionProvider(Logger logger, IEyesJsExecutor executor, IWebElement scrollRootElement)
            : base(logger, executor, scrollRootElement)
        {
        }

        protected override string SetPositionCommandFormat => "window.scrollTo({0},{1}); return (window.scrollX+';'+window.scrollY);";
    }
}
