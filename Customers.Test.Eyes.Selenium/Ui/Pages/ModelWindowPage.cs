namespace Applitools.Ui.Pages
{
    using System;
    using System.Threading;
    using Applitools.Utils;
    using OpenQA.Selenium;

    /// <summary>
    /// A model window page.
    /// </summary>
    public class ModelWindowPage : ModelPage
    {
        private const string PageReadyIndicator_ = "img.currentWindowImage[src]";
        private const string WindowGalleryButtonId_ = "preferencesButton";

        /// <summary>
        /// Creates a new ModelWindowPage instance.
        /// </summary>
        /// <param name="driver"></param>
        public ModelWindowPage(IWebDriver driver)
            : base(driver)
        {
        }

        /// <summary>
        /// Opens the model window page showing the root window of the specified
        /// model id and returns its page object (when ready).
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="appBaseUrl"></param>
        /// <param name="modelId"></param>
        /// <param name="testKey"></param>
        /// <returns></returns>
        public static ModelWindowPage Open(
            IWebDriver driver, Uri appBaseUrl, string modelId, string testKey)
        {
            ArgumentGuard.NotNull(appBaseUrl, nameof(appBaseUrl));
            ArgumentGuard.NotNull(modelId, nameof(modelId));
            ArgumentGuard.NotNull(testKey, nameof(testKey));

            driver.Url = new Url(appBaseUrl).SubpathElement("app/models/" + modelId)
                .QueryElement("test", testKey)
                .QueryElement("accountId", Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY"))
                .ToString();

            ModelWindowPage windowPage = new ModelWindowPage(driver);
            windowPage.WaitUntilReady();
            return windowPage;
        }

        /// <summary>
        /// Clicks on the middle of the control, and moves the cursor to the
        /// top left of the screen to avoid hover related problems.
        /// </summary>
        /// <param name="control"></param>
        /// <returns>Returns this page.</returns>
        public ModelWindowPage ClickControl(ModelControl control)
        {
            ArgumentGuard.NotNull(control, "control");

            SeleniumUtils.ClickAndWait(
                Driver, 
                Driver.FindElement(By.CssSelector(".ImageContainer")),
                control.Center, 
                By.CssSelector(PageReadyIndicator_));
            return this;
        }

        /// <summary>
        /// Changes to bird view by clicking the bird view button.
        /// </summary>
        /// <returns></returns>
        public ModelBirdViewPage ClickBirdViewButton()
        {
            Driver.FindElement(By.CssSelector(ModelViewButtonSelector)).Click();
            ModelBirdViewPage birdViewPage = new ModelBirdViewPage(Driver);
            birdViewPage.WaitUntilReady();
            return birdViewPage;
        }

        /// <summary>
        /// Opens the model window gallery by clicking the window gallery button.
        /// </summary>
        /// <returns></returns>
        public ModelWindowGalleryPage ClickGalleryButton()
        {
            SeleniumUtils.ClickAndMove(
                Driver, Driver.FindElement(By.Id(WindowGalleryButtonId_)));
            ModelWindowGalleryPage galleryPage = new ModelWindowGalleryPage(Driver);
            galleryPage.WaitUntilReady();
            return galleryPage;
        }

        public new ModelWindowPage ZoomByMouseWheel(int delta)
        {
            return (ModelWindowPage)base.ZoomByMouseWheel(delta);
        }

        public new ModelWindowPage ZoomByMenuButton(int delta)
        {
            return (ModelWindowPage)base.ZoomByMenuButton(delta);
        }

        public override void WaitUntilReady()
        {
            SeleniumUtils.WaitForElement(Driver, By.CssSelector(PageReadyIndicator_));
            Thread.Sleep(10000);
        }
    }
}
