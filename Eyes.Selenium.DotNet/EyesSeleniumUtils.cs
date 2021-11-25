using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace Applitools.Selenium
{
    static class EyesSeleniumUtils
    {
        private const string JSSetOverflow_ =
            "var origOF = arguments[0].style.overflow;" +
            "arguments[0].style.overflow = '{0}';" +
            "if ('{0}'.toUpperCase() === 'HIDDEN' && origOF.toUpperCase() !== 'HIDDEN') arguments[0].setAttribute('data-applitools-original-overflow',origOF);" +
            "return origOF;";

        private const string JSGetViewportSize_ =
            "var width = undefined, height = undefined; " +
            "if (window.innerWidth) {width = window.innerWidth; height = window.innerHeight;} " +
            "else if (document.documentElement && document.documentElement.clientWidth) " +
            "{width = document.documentElement.clientWidth; height = document.documentElement.clientHeight;} " +
            "else if (document.body.clientWidth) {width = document.body.clientWidth; height = document.body.clientHeight;} " +
            "return (width + ';' + height);";

        private const int SetOverflowWaitMS_ = 200;

        private static Size GetViewportSize_(Logger logger, IJavaScriptExecutor jsExecutor)
        {
            if (jsExecutor.ExecuteScript(JSGetViewportSize_) is string sizeStr)
            {
                try
                {
                    string[] widthAndHeight = sizeStr.Split(';');
                    int width = (int)Math.Round(Convert.ToDouble(widthAndHeight[0]));
                    int height = (int)Math.Round(Convert.ToDouble(widthAndHeight[1]));
                    return new Size(width, height);
                }
                catch
                {
                    logger.Log(TraceLevel.Error, Stage.General, StageType.Parse, new { sizeStr });
                    throw;
                }
            }

            return Size.Empty;
        }

        public static Size GetViewportSize(Logger logger, IWebDriver driver)
        {
            Size size;
            if (driver is EyesWebDriver eyesWebDriver && eyesWebDriver.GetFrameChain().Count > 0)
            {
                size = eyesWebDriver.GetDefaultContentViewportSize(true);
            }
            else
            {
                size = GetViewportSize_(logger, (IJavaScriptExecutor)driver);
            }
            return size;
        }

        public static Size GetViewportSizeOrDisplaySize(Logger logger, string testId, IWebDriver driver)
        {
            Size size;
            try
            {
                size = GetViewportSize(logger, driver);
                if (size.Width > 0 && size.Height > 0)
                {
                    return size;
                }

                logger.Log(TraceLevel.Warn, testId, Stage.General, new { message = "Either the Width or Height value returned zero." });
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(logger, Stage.General, ex, testId);
            }

            logger.Log(TraceLevel.Notice, testId, Stage.General, new { message = "Using browser size." });
            size = driver.Manage().Window.Size;

            try
            {
                if (IsLandscapeOrientation(driver) && size.Height > size.Width)
                {
                    size = new Size(size.Height, size.Width);
                }
            }
            catch (WebDriverException)
            {
                // Not every WebDriver supports querying for orientation.
            }
            logger.Log(TraceLevel.Info, testId, Stage.General, StageType.Complete, new { size });

            return size;
        }

        public static Func<IWebDriver, bool> IsLandscapeOrientation = IsLandscapeOrientationImpl_;

        /// <summary>
        /// Returns whether the device represented by <paramref name="driver"/> is in landscape mode or not.
        /// </summary>
        /// <param name="driver">The driver for which to check the orientation.</param>
        /// <returns>True if this is a mobile device and is in landscape orientation, or False otherwise.</returns>
        private static bool IsLandscapeOrientationImpl_(IWebDriver driver)
        {
            if (driver is IRotatable rotatable)
            {
                try
                {
                    ScreenOrientation orientation = rotatable.Orientation;
                    return orientation == ScreenOrientation.Landscape;
                }
                catch (Exception e)
                {
                    throw new EyesDriverOperationException("Failed to get orientation!", e);
                }
            }
            return false;
        }

        internal static string GetElementId(IWebElement element)
        {
            FieldInfo fi = typeof(RemoteWebElement).GetField("elementId", BindingFlags.NonPublic | BindingFlags.Instance);
            return (string)fi.GetValue(element);
        }

        internal static string GetElementIdForDictionary(IWebElement element, RemoteWebDriver remoteWebDriver)
        {
            return GetElementId(element) + "_" + remoteWebDriver.SessionId;
        }

        public static void SetViewportSize(Logger logger, IWebDriver driver, RectangleSize size)
        {
            EyesSeleniumUtilsCommon.SetViewportSize(logger, driver, size,
                d => d.Manage().Window.Position = new Point(0, 0),
                (d, s) => d.Manage().Window.Size = s,
                d => d.Manage().Window.Size);
        }

        /// <summary>
        /// Sets the overflow of the given element, and returns the previous overflow value.
        /// </summary>
        /// <param name="overflow">The overflow to set.</param>
        /// <param name="jsExecutor"></param>
        /// <param name="rootElement">The element for which to set the overflow.</param>
        /// <returns>The previous overflow value.</returns>
        public static string SetOverflow(string overflow, IEyesJsExecutor jsExecutor, IWebElement rootElement)
        {
            ArgumentGuard.NotNull(jsExecutor, nameof(jsExecutor));
            ArgumentGuard.NotNull(rootElement, nameof(rootElement));

            var result = jsExecutor.ExecuteScript(JSSetOverflow_.Fmt(overflow), rootElement);
            Thread.Sleep(SetOverflowWaitMS_);
            return result as string;
        }

        public static Rectangle GetElementBounds(IWebElement element)
        {
            Rectangle r;
            if (element is EyesRemoteWebElement eyesElement)
            {
                r = eyesElement.GetClientBounds();
            }
            else
            {
                r = new Rectangle(element.Location, element.Size);
            }
            return r;
        }

        public static IWebElement FindFrameByFrameCheckTarget(Fluent.ISeleniumFrameCheckTarget frameTarget, IWebDriver driver)
        {
            if (frameTarget.GetFrameIndex().HasValue)
            {
                return driver.FindElement(By.XPath($"IFRAME[{frameTarget.GetFrameIndex().Value}]"));
            }

            string nameOrId = frameTarget.GetFrameNameOrId();
            if (nameOrId != null)
            {
                IList<IWebElement> byId = driver.FindElements(By.Id(nameOrId));
                if (byId.Count > 0)
                {
                    return byId[0];
                }
                return driver.FindElement(By.Name(nameOrId));
            }

            IWebElement reference = frameTarget.GetFrameReference();
            if (reference != null)
            {
                return reference;
            }

            By selector = frameTarget.GetFrameSelector();
            if (selector != null)
            {
                return driver.FindElement(selector);
            }

            return null;
        }

        public static IWebElement GetDefaultRootElement(IWebDriver driver, Logger logger = null)
        {
            var result = EyesSeleniumUtilsCommon.GetDefaultRootElement<IWebDriver, IWebElement>(driver, GetHtmlAndBodyElements, e => e.TagName, logger);
            return result;
        }

        private static void GetHtmlAndBodyElements(IWebDriver driver, out IWebElement html, out ReadOnlyCollection<IWebElement> bodies)
        {
            html = driver.FindElement(By.TagName("html"));
            bodies = driver.FindElements(By.TagName("body"));
        }

        public static IWebElement GetCurrentFrameScrollRootElement(EyesWebDriver driver, IWebElement userDefinedSRE)
        {
            IWebElement scrollRootElement = TryGetCurrentFrameScrollRootElement(driver);
            if (scrollRootElement == null)
            {
                scrollRootElement = userDefinedSRE ?? GetDefaultRootElement(driver);
            }
            return scrollRootElement;
        }

        public static IWebElement TryGetCurrentFrameScrollRootElement(EyesWebDriver driver)
        {
            FrameChain fc = driver.GetFrameChain().Clone();
            Frame currentFrame = fc.Peek();
            IWebElement scrollRootElement = null;
            if (currentFrame != null)
            {
                scrollRootElement = currentFrame.ScrollRootElement;
            }

            return scrollRootElement;
        }

        public static Rectangle GetVisibleElementBounds(IWebElement element)
        {
            Rectangle r;
            if (element is EyesRemoteWebElement eyesElement)
            {
                r = eyesElement.GetVisibleElementRect();
            }
            else
            {
                r = new Rectangle(element.Location, element.Size);
            }
            return r;
        }

        public static string GetUserAgent(this IWebDriver driver)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)driver;
            string ua = (string)executor.ExecuteScript("return navigator.userAgent;");
            return ua;
        }
    }
}
