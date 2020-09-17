namespace Applitools
{
    using Utils;

    public class AppOutputWithScreenshot
    {
        public AppOutputWithScreenshot(AppOutput appOutput, EyesScreenshot screenshot)
        {
            ArgumentGuard.NotNull(appOutput, nameof(appOutput));
            if (appOutput.ScreenshotUrl == null)
            {
                ArgumentGuard.NotNull(screenshot, nameof(screenshot));
            }

            AppOutput = appOutput;
            Screenshot = screenshot;
        }

        public AppOutput AppOutput { get; set; }

        public EyesScreenshot Screenshot { get; set; }
    }
}
