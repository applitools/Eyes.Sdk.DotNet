using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using Applitools.Selenium.Capture;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Newtonsoft.Json;
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

        private static readonly string DOM_SCRIPTS_WRAPPER = "return ({0})({1});";
        internal static TimeSpan CAPTURE_TIMEOUT = TimeSpan.FromMinutes(5);

        private static Size GetViewportSize_(Logger logger, IJavaScriptExecutor jsExecutor)
        {
            if (jsExecutor.ExecuteScript(JSGetViewportSize_) is string jsonSize)
            {
                try
                {
                    string[] widthAndHeight = jsonSize.Split(';');
                    int width = (int)Math.Round(Convert.ToDouble(widthAndHeight[0]));
                    int height = (int)Math.Round(Convert.ToDouble(widthAndHeight[1]));
                    return new Size(width, height);
                }
                catch
                {
                    logger.Log("Error: Failed parsing input size string: '{0}'", jsonSize);
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
            logger.Verbose("viewport size: {0}", size);
            return size;
        }

        public static Size GetViewportSizeOrDisplaySize(Logger logger, IWebDriver driver)
        {
            Size size;
            try
            {
                size = GetViewportSize(logger, driver);
                if (size.Width > 0 && size.Height > 0)
                {
                    return size;
                }

                logger.Log("Either the Width or Height value returned zero.");
            }
            catch (Exception ex)
            {
                logger.Log("inner width / height not supported: {0}", ex.Message);
            }

            logger.Log("Using browser size.");
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
            logger.Verbose("Done! Size: " + size);

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

        private static bool SetBrowserSize_(Logger logger, IWebDriver driver, Size requiredSize)
        {
            const int SLEEP = 1000;
            const int RETRIES = 3;

            var retriesLeft = RETRIES;
            Size currentSize;
            do
            {
                logger.Verbose("Trying to set the browser size to: {0}", requiredSize);
                driver.Manage().Window.Size = requiredSize;
                Thread.Sleep(SLEEP);
                currentSize = driver.Manage().Window.Size;
                logger.Verbose("Current browser size: {0}", currentSize);
            }
            while (--retriesLeft > 0 && currentSize != requiredSize);

            return currentSize == requiredSize;
        }

        public static bool SetBrowserSizeByViewportSize(Logger logger, IWebDriver driver,
            Size actualViewportSize, Size requiredViewportSize)
        {
            var browserSize = driver.Manage().Window.Size;
            logger.Verbose("Current browsers size: {0}", browserSize);
            Size requiredBrowserSize = new Size(
                    browserSize.Width + (requiredViewportSize.Width - actualViewportSize.Width),
                    browserSize.Height + (requiredViewportSize.Height - actualViewportSize.Height));
            return SetBrowserSize_(logger, driver, requiredBrowserSize);
        }

        public static void SetViewportSize(Logger logger, IWebDriver driver, RectangleSize size)
        {
            ArgumentGuard.NotNull(size, nameof(size));

            logger.Verbose("SetViewportSize({0})", size);

            Size requiredSize = new Size(size.Width, size.Height);
            Size actualViewportSize = GetViewportSize(logger, driver);

            logger.Verbose("initial viewport size: {0}", actualViewportSize);

            if (actualViewportSize == requiredSize)
            {
                logger.Verbose("Required size already set.");
                return;
            }

            actualViewportSize = SetViewportSizeAttempt1(logger, driver, requiredSize, actualViewportSize);
            if (actualViewportSize == requiredSize)
            {
                return;
            }

            actualViewportSize = SetViewportSizeAttempt2(logger, driver, requiredSize, actualViewportSize);
            if (actualViewportSize == requiredSize)
            {
                return;
            }
            bool success = SetViewportSizeAttempt3(logger, driver, requiredSize, ref actualViewportSize);
            if (success)
            {
                return;
            }

            throw new EyesException("Failed to set viewport size!");
        }

        private static bool SetViewportSizeAttempt3(Logger logger, IWebDriver driver, Size requiredSize, ref Size actualViewportSize)
        {
            bool success = false;
            const int MAX_DIFF = 3;

            int widthDiff = actualViewportSize.Width - requiredSize.Width;
            int widthStep = widthDiff > 0 ? -1 : 1; // -1 for smaller size, 1 for larger

            int heightDiff = actualViewportSize.Height - requiredSize.Height;
            int heightStep = heightDiff > 0 ? -1 : 1;

            Size browserSize = driver.Manage().Window.Size;

            int areaDiff = Math.Sign(actualViewportSize.Area() - requiredSize.Area());

            int currWidthChange = 0;
            int currHeightChange = 0;
            // We try the zoom workaround only if size difference is reasonable.
            if (Math.Abs(widthDiff) <= MAX_DIFF && Math.Abs(heightDiff) <= MAX_DIFF)
            {
                logger.Verbose("Trying workaround for zoom...");
                int retriesLeft = Math.Abs((widthDiff == 0 ? 1 : widthDiff) * (heightDiff == 0 ? 1 : heightDiff)) * 2;
                Size lastRequiredBrowserSize = Size.Empty;
                do
                {
                    logger.Verbose("Retries left: {0}", retriesLeft);
                    currWidthChange = ModifySizeChange(requiredSize.Width, actualViewportSize.Width, widthDiff, widthStep, currWidthChange);
                    currHeightChange = ModifySizeChange(requiredSize.Height, actualViewportSize.Height, heightDiff, heightStep, currHeightChange);

                    Size requiredBrowserSize = new Size(browserSize.Width + currWidthChange, browserSize.Height + currHeightChange);

                    if (requiredBrowserSize.Equals(lastRequiredBrowserSize))
                    {
                        logger.Verbose("Browser size is as required but viewport size does not match!");
                        logger.Verbose("Browser size: {0}, Viewport size: {1}", requiredBrowserSize, actualViewportSize);
                        logger.Verbose("Stopping viewport size attempts.");
                        break;
                    }

                    SetBrowserSize_(logger, driver, requiredBrowserSize);
                    lastRequiredBrowserSize = requiredBrowserSize;

                    actualViewportSize = GetViewportSize(logger, driver);
                    logger.Verbose("Current viewport size: {0}", actualViewportSize);

                    if (actualViewportSize == requiredSize)
                    {
                        success = true;
                        break;
                    }
                } while ((Math.Abs(currWidthChange) <= Math.Abs(widthDiff) ||
                    Math.Abs(currHeightChange) <= Math.Abs(heightDiff)) &&
                     (--retriesLeft > 0));

                logger.Verbose("Zoom workaround failed.");
            }

            if (!success)
            {
                bool isRoundingError =
                    Math.Abs(actualViewportSize.Height - requiredSize.Height) < MAX_DIFF &&
                    Math.Abs(actualViewportSize.Width - requiredSize.Width) < MAX_DIFF;
                throw new EyesSetViewportSizeException(actualViewportSize, isRoundingError);
            }

            return success;
        }

        private static int ModifySizeChange(int requiredSize, int actualViewportSize, int sizeDiff, int step, int currChange)
        {
            // We specifically use "<=" (and not "<"), so to give an extra resize attemp
            // in addition to reaching the diff, due to floating point issues.
            if (Math.Abs(currChange) <= Math.Abs(sizeDiff) &&
                actualViewportSize != requiredSize)
            {
                return currChange + step;
            }
            return currChange;
        }

        private static Size SetViewportSizeAttempt2(Logger logger, IWebDriver driver, Size requiredSize, Size actualViewportSize)
        {

            // Additional attempt. This Solves the "maximized browser" bug
            // (border size for maximized browser sometimes different than
            // non-maximized, so the original browser size calculation is
            // wrong).
            logger.Verbose("Trying workaround for maximization...");
            SetBrowserSizeByViewportSize(logger, driver, actualViewportSize, requiredSize);

            actualViewportSize = GetViewportSize(logger, driver);
            logger.Verbose("Current viewport size: {0}", actualViewportSize);
            return actualViewportSize;
        }

        private static Size SetViewportSizeAttempt1(Logger logger, IWebDriver driver, Size requiredSize, Size actualViewportSize)
        {
            // Move the window to (0,0) to have the best chance to be able to
            // set the viewport size as requested.
            if (Eyes.moveWindow_)
            {
                driver.Manage().Window.Position = new Point(0, 0);
            }

            SetBrowserSizeByViewportSize(logger, driver, actualViewportSize, requiredSize);

            actualViewportSize = GetViewportSize(logger, driver);

            logger.Verbose("Current viewport size: {0}", actualViewportSize);
            return actualViewportSize;
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
                System.Collections.Generic.IList<IWebElement> byId = driver.FindElements(By.Id(nameOrId));
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

        public static Point ParseLocationString(object position)
        {
            var xy = position.ToString().Split(';');
            if (xy.Length != 2 ||
                !float.TryParse(xy[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float x) ||
                !float.TryParse(xy[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float y))
            {
                throw new EyesException($"Could not get scroll position! position: {position}");
            }

            return new Point((int)Math.Ceiling(x), (int)Math.Ceiling(y));
        }

        public static IWebElement GetDefaultRootElement(IWebDriver driver, Logger logger = null)
        {
            IWebElement chosenElement;
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;

            IWebElement scrollingElement;
            try
            {
                scrollingElement = (IWebElement)jsExecutor.ExecuteScript("return document.scrollingElement");
            }
            catch (Exception e)
            {
                logger?.Log("Error: {0}", e);
                scrollingElement = null;
            }

            IWebElement html = driver.FindElement(By.TagName("html"));
            ReadOnlyCollection<IWebElement> bodies = driver.FindElements(By.TagName("body"));

            if (scrollingElement != null && CanScrollVertically(jsExecutor, scrollingElement))
            {
                // If document.scrollingElement exists and can scroll vertically then it's the element we are looking for
                chosenElement = scrollingElement;
            }
            else if (bodies.Count == 0)
            {
                chosenElement = html;
            }
            else
            {
                IWebElement body = bodies[0];
                bool scrollableHtml = CanScrollVertically(jsExecutor, html);
                bool scrollableBody = CanScrollVertically(jsExecutor, body);

                // If only one of the elements is scrollable, we return the scrollable one
                if (scrollableHtml && !scrollableBody)
                {
                    chosenElement = html;
                }
                else if (!scrollableHtml && scrollableBody)
                {
                    chosenElement = body;
                }
                else if (scrollingElement != null)
                {
                    // If both of the elements are scrollable or both aren't scrollable, we choose document.scrollingElement which is always one of them
                    chosenElement = scrollingElement;
                }
                else
                {
                    // If document.scrollingElement, we choose html
                    chosenElement = html;
                }
            }
            logger?.Verbose("Chosen default root element is {0}", chosenElement.TagName);
            return chosenElement;
        }

        private static bool CanScrollVertically(IJavaScriptExecutor jsExecutor, IWebElement element)
        {
            return (bool)jsExecutor.ExecuteScript("return arguments[0].scrollHeight > arguments[0].clientHeight", element);
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

        internal static string RunDomScript(Logger logger, EyesWebDriver driver,
            string domScript, object domScriptArguments, object pollingScriptArguments, string pollingScript)
        {
            logger.Verbose("Starting dom extraction");
            if (domScriptArguments == null)
            {
                domScriptArguments = new { };
            }

            string domScriptWrapped = string.Format(DOM_SCRIPTS_WRAPPER, domScript, JsonConvert.SerializeObject(domScriptArguments));
            string pollingScriptWrapped = string.Format(DOM_SCRIPTS_WRAPPER, pollingScript, JsonConvert.SerializeObject(pollingScriptArguments));

            try
            {
                string resultAsString = (string)driver.ExecuteScript(domScriptWrapped);
                CaptureStatus scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                CaptureStatusEnum status = scriptResponse.Status;
                Stopwatch stopwatch = Stopwatch.StartNew();
                while (status == CaptureStatusEnum.WIP && stopwatch.Elapsed < CAPTURE_TIMEOUT)
                {
                    Thread.Sleep(200);
                    logger.Verbose("Dom script polling...");
                    resultAsString = (string)driver.ExecuteScript(pollingScriptWrapped);
                    scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                    status = scriptResponse.Status;
                }

                if (status == CaptureStatusEnum.ERROR)
                {
                    throw new EyesException("DomSnapshot Error: " + scriptResponse.Error);
                }

                if (stopwatch.Elapsed > CAPTURE_TIMEOUT)
                {
                    throw new EyesException("DOM capture timeout.");
                }

                if (status == CaptureStatusEnum.SUCCESS)
                {
                    return scriptResponse.Value.ToString();
                }

                StringBuilder value = new StringBuilder();
                string chunk;
                while (status == CaptureStatusEnum.SUCCESS_CHUNKED && !scriptResponse.Done && stopwatch.Elapsed < CAPTURE_TIMEOUT)
                {
                    logger.Verbose("Dom script chunks polling...");
                    chunk = scriptResponse.Value.ToString();
                    value.Append(chunk);
                    resultAsString = (string)driver.ExecuteScript(pollingScriptWrapped);
                    scriptResponse = JsonConvert.DeserializeObject<CaptureStatus>(resultAsString);
                    status = scriptResponse.Status;
                }

                if (status == CaptureStatusEnum.ERROR)
                {
                    throw new EyesException("DomSnapshot Error: " + scriptResponse.Error);
                }

                if (stopwatch.Elapsed > CAPTURE_TIMEOUT)
                {
                    throw new EyesException("Domsnapshot Timed out");
                }

                chunk = scriptResponse.Value.ToString();
                value.Append(chunk);
                return value.ToString();
            }
            finally
            {
                logger.Verbose("Finished dom extraction");
            }
        }
    }
}
