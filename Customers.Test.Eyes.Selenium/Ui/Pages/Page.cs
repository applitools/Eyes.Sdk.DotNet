namespace Applitools.Ui.Pages
{
    using Applitools.Utils;
    using OpenQA.Selenium;

    public abstract class Page
    {
        /// <summary>
        /// Creates a new Page instance.
        /// </summary>
        /// <param name="driver"></param>
        public Page(IWebDriver driver)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));

            Driver = driver;
        }

        protected IWebDriver Driver { get; private set; }

        /// <summary>
        /// Waits until the page is ready.
        /// </summary>
        public abstract void WaitUntilReady();
    }
}
