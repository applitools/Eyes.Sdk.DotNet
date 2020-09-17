namespace Applitools.Ui.Pages
{
    using System;
    using System.Threading;
    using Applitools.Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// A window gallery page.
    /// </summary>
    public class ModelWindowGalleryPage : Page
    {
        /// <summary>
        ///  Creates a new ModelWindowGalleryPage instance.
        /// </summary>
        /// <param name="driver"></param>
        public ModelWindowGalleryPage(IWebDriver driver)
            : base(driver)
        {
        }

        public override void WaitUntilReady()
        {
            SeleniumUtils.WaitForEvent(Driver, "windowGalleryReady");
        }

        /// <summary>
        /// Clicks the image of the window of the specified id (closes the
        /// gallery page), and returns the page that opened this page.
        /// </summary>
        public void ClickImage(string windowId)
        {
            ArgumentGuard.NotNull(windowId, nameof(windowId));

            SeleniumUtils.ClickAndMove(
                Driver, Driver.FindElement(By.CssSelector(CssSelector_(windowId))));
            Thread.Sleep(500);
        }

        /// <summary>
        /// Hovers over the image of the window of the specified id.
        /// </summary>
        /// <param name="windowId"></param>
        /// <returns></returns>
        public ModelWindowGalleryPage HoverOverImage(string windowId)
        {
            ArgumentGuard.NotNull(windowId, nameof(windowId));

            SeleniumUtils.HoverOverElement(
                Driver, Driver.FindElement(By.CssSelector(CssSelector_(windowId))));
            return this;
        }

        /// <summary>
        /// Returns a CSS selector for the <c>img</c> element holding the image of the
        /// window of the specified id.
        /// </summary>
        private static string CssSelector_(string windowId)
        {
            ArgumentGuard.NotNull(windowId, nameof(windowId));
          
            /// Do not remove the space after ".windowList"!
            string windowDataIdSelector = "[data-id=\"" + windowId + "\"]";
            return ".windowList " + windowDataIdSelector + " img";
        }
    }
}
