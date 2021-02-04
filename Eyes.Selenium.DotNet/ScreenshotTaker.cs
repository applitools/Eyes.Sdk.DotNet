namespace Applitools.Selenium
{
    using System;
    using Utils;

    internal class ScreenshotTaker
    {
        #region Fields

        private HttpRestClient httpClient_;
    
        #endregion

        #region Constructors

        public ScreenshotTaker(Logger logger, Uri driverServerUri, string driverSessionId)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotEmpty(driverSessionId, nameof(driverSessionId));

            var url = new Url(driverServerUri)
                .SubpathElement("session")
                .SubpathElement(driverSessionId)
                .SubpathElement("screenshot");
            httpClient_ = new HttpRestClient(url);
            Logger = logger;
        }

        #endregion

        #region Properties

        protected Logger Logger { get; private set; }

        #endregion

        #region Methods

        public string GetScreenshot()
        {
            try
            {
                // Performing the call to get the screenshot
                var response = httpClient_.Get(string.Empty, "application/json");
                var screenshot = response.DeserializeBody<Screenshot_>(true);

                return screenshot.Value;
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.Check, StageType.CaptureScreenshot, ex);
                throw;
            }
        }

        #endregion

        #region Classes

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance", 
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Serialization")]
        private class Screenshot_
        {
            public string Value { get; set; }
        }

        #endregion
    }
}
