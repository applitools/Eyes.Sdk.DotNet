using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Threading;
using Applitools.Selenium.Common;
using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools.Selenium
{
    static class EyesSeleniumUtilsCommon
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


        private static readonly string JS_GET_COMPUTED_STYLE_FORMATTED_STR =
          "var elem = arguments[0]; " +
          "var styleProp = '{0}'; " +
          "if (window.getComputedStyle) {{ " +
              "return window.getComputedStyle(elem, null)" +
              ".getPropertyValue(styleProp);" +
          "}} else if (elem.currentStyle) {{ " +
              "return elem.currentStyle[styleProp];" +
          "}} else {{ " +
              "return null;" +
          "}}";

        private static readonly string JS_GET_BORDER_WIDTHS_STR =
            "if (window.getComputedStyle) { " +
                "var computedStyle = window.getComputedStyle(elem, null);" +
                "retval += computedStyle.getPropertyValue('border-left-width') + ',' +" +
                          "computedStyle.getPropertyValue('border-top-width') + ',' + " +
                          "computedStyle.getPropertyValue('border-right-width') + ',' + " +
                          "computedStyle.getPropertyValue('border-bottom-width');" +
            "} else if (elem.currentStyle) { " +
                "retval += elem.currentStyle['border-left-width'] + ',' + " +
                          "elem.currentStyle['border-top-width'] + ',' + " +
                          "elem.currentStyle['border-right-width'] + ',' + " +
                          "elem.currentStyle['border-bottom-width'];" +
            "} else { " +
                "retval += '0,0,0,0'" +
            "}";

        private static readonly string JS_GET_BORDER_WIDTHS =
            "var elem = arguments[0]; " +
            "var retval = ''; " +
            JS_GET_BORDER_WIDTHS_STR +
            "return (retval.length === 0) ? null : retval;";

        private static readonly string JS_GET_SIZE_AND_BORDER_WIDTHS =
            "var elem = arguments[0]; " +
            "var retval = elem.clientWidth + ',' + elem.clientHeight + ','; " +
            JS_GET_BORDER_WIDTHS_STR +
            "return retval;";

        private static readonly string JS_GET_SCROLL_LEFT = "return arguments[0].scrollLeft;";
        private static readonly string JS_GET_SCROLL_TOP = "return arguments[0].scrollTop;";
        private static readonly string JS_GET_SCROLL_WIDTH = "return arguments[0].scrollWidth;";
        private static readonly string JS_GET_SCROLL_HEIGHT = "return arguments[0].scrollHeight;";

        private static readonly string JS_GET_SCROLL_POSITION = "return arguments[0].scrollLeft + ',' + arguments[0].scrollTop;";
        private static readonly string JS_GET_SCROLL_SIZE = "return arguments[0].scrollWidth + ',' + arguments[0].scrollHeight;";
        private static readonly string JS_GET_OFFSET_POSITION = "return arguments[0].offsetLeft + ',' + arguments[0].offsetTop;";
        private static readonly string JS_GET_CLIENT_WIDTH = "return arguments[0].clientWidth;";
        private static readonly string JS_GET_CLIENT_HEIGHT = "return arguments[0].clientHeight;";

        private static readonly string JS_GET_CLIENT_SIZE = "return arguments[0].clientWidth + ',' + arguments[0].clientHeight;";

        private static readonly string JS_GET_ENTIRE_ELEMENT_SIZE =
            "var width = Math.max(arguments[0].clientWidth, arguments[0].scrollWidth);" +
            "var height = Math.max(arguments[0].clientHeight, arguments[0].scrollHeight);" +
            "return (width + ';' + height);";

        private static readonly string JS_SCROLL_TO_FORMATTED_STR =
            "arguments[0].scrollLeft = {0};" +
            "arguments[0].scrollTop = {1};";

        private static readonly string JS_GET_OVERFLOW = "return arguments[0].style.overflow;";

        private static readonly string JS_GET_BOUNDING_CLIENT_RECT =
            "var bcr = arguments[0].getBoundingClientRect();" +
            "return bcr.left + ';' + bcr.top + ';' + bcr.width + ';' + bcr.height;";

        private static readonly string JS_GET_BOUNDING_CLIENT_RECT_WITHOUT_BORDERS =
            "var el = arguments[0];" +
            "var bcr = el.getBoundingClientRect();" +
            "return (bcr.left + el.clientLeft) + ';' + (bcr.top + el.clientTop) + ';' + el.clientWidth + ';' + el.clientHeight;";

        private static readonly string JS_GET_CLIENT_VISUAL_OFFSET =
            "var el = arguments[0];" +
            "var bcr = el.getBoundingClientRect();" +
            "return (bcr.left + el.clientLeft) +';'+ (bcr.top + el.clientTop);";

        //private readonly string JS_GET_OFFSET_POSITON = "return arguments[0].offsetLeft+','+arguments[0].offsetTop;";

        private static readonly string JS_GET_VISIBLE_ELEMENT_RECT = @"function getVisibleElementRect(el) {
  var intersected = el.getBoundingClientRect()
  el = el.parentElement;
  if (el != null && el.tagName === 'HTML'){
    el = el.ownerDocument.defaultView.frameElement
  }
  while (el != null) {
    var bcr = el.getBoundingClientRect()
    var cr = {
      left: bcr.left + el.clientLeft,
      top: bcr.top + el.clientTop,
      width: el.clientWidth,
      height: el.clientHeight,
    }
    if (el.tagName === 'IFRAME'){
      intersected.left += cr.left
      intersected.top += cr.top
    } 
    
    var tempIntersected = intersect(cr, intersected)
    if (tempIntersected.height<intersected.height) {
      if (window.getComputedStyle(el)['overflowY'] == 'visible'){
        tempIntersected.height = intersected.height
      }
    }
    if (tempIntersected.width < intersected.width)
    {
      if (window.getComputedStyle(el)['overflowX'] == 'visible')
      {
          tempIntersected.width = intersected.width
      }
    }
    intersected = tempIntersected

    el = el.parentElement;
    if (el != null && el.tagName === 'HTML'){
      el = el.ownerDocument.defaultView.frameElement
    }
  }
  return intersected.left+';'+intersected.top+';'+intersected.width+';'+intersected.height;
  function intersect(rect1, rect2) {
    var intersectionLeft = rect1.left >= rect2.left ? rect1.left : rect2.left
    var intersectionTop = rect1.top >= rect2.top ? rect1.top : rect2.top
    var rect1Right = rect1.left + rect1.width
    var rect2Right = rect2.left + rect2.width
    var intersectionRight = rect1Right <= rect2Right ? rect1Right : rect2Right
    var intersectionWidth = intersectionRight - intersectionLeft
    var rect1Bottom = rect1.top + rect1.height
    var rect2Bottom = rect2.top + rect2.height
    var intersectionBottom = rect1Bottom <= rect2Bottom ? rect1Bottom : rect2Bottom
    var intersectionHeight = intersectionBottom - intersectionTop
    return {
      left: intersectionLeft,
      top: intersectionTop,
      width: intersectionWidth,
      height: intersectionHeight,
    }
  }
}
return getVisibleElementRect(arguments[0])";

        private const int SetOverflowWaitMS_ = 200;
        internal static bool moveWindow_ = true;

        private static Size GetViewportSize_(Logger logger, IEyesJsExecutor jsExecutor)
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

        public static Size GetViewportSize<IWebDriver>(Logger logger, IWebDriver driver)
        {
            Size size;

            if (driver is IEyesWebDriverCommon eyesWebDriver && eyesWebDriver.GetFrameChainLength() > 0)
            {
                size = eyesWebDriver.GetDefaultContentViewportSize(true);
            }
            else
            {
                size = GetViewportSize_(logger, (IEyesJsExecutor)driver);
            }
            return size;
        }

        internal static void SetViewportSize<IWebDriver>(Logger logger, IWebDriver driver, RectangleSize size,
            Action<IWebDriver> moveBrowserToOrigin,
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
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

            actualViewportSize = SetViewportSizeAttempt1(logger, driver, requiredSize, actualViewportSize, moveBrowserToOrigin, setBrowserSize, getBrowserSize);
            if (actualViewportSize == requiredSize)
            {
                return;
            }

            actualViewportSize = SetViewportSizeAttempt2(logger, driver, requiredSize, actualViewportSize, setBrowserSize, getBrowserSize);
            if (actualViewportSize == requiredSize)
            {
                return;
            }
            bool success = SetViewportSizeAttempt3(logger, driver, requiredSize, ref actualViewportSize, setBrowserSize, getBrowserSize);
            if (success)
            {
                return;
            }

            throw new EyesException("Failed to set viewport size!");
        }

        private static bool SetViewportSizeAttempt3<IWebDriver>(
            Logger logger, IWebDriver driver, Size requiredSize, ref Size actualViewportSize, 
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
        {
            bool success = false;
            const int MAX_DIFF = 3;

            int widthDiff = actualViewportSize.Width - requiredSize.Width;
            int widthStep = widthDiff > 0 ? -1 : 1; // -1 for smaller size, 1 for larger

            int heightDiff = actualViewportSize.Height - requiredSize.Height;
            int heightStep = heightDiff > 0 ? -1 : 1;

            Size browserSize = getBrowserSize(driver);

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

                    SetBrowserSize_(logger, driver, requiredBrowserSize, setBrowserSize, getBrowserSize);
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

        public static bool SetBrowserSizeByViewportSize<IWebDriver>(Logger logger, IWebDriver driver,
            Size actualViewportSize, Size requiredViewportSize,
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
        {
            var browserSize = getBrowserSize(driver);
            logger.Verbose("Current browsers size: {0}", browserSize);
            Size requiredBrowserSize = new Size(
                    browserSize.Width + (requiredViewportSize.Width - actualViewportSize.Width),
                    browserSize.Height + (requiredViewportSize.Height - actualViewportSize.Height));
            return SetBrowserSize_(logger, driver, requiredBrowserSize, setBrowserSize, getBrowserSize);
        }

        private static bool SetBrowserSize_<IWebDriver>(Logger logger, IWebDriver driver, Size requiredSize,
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
        {
            const int SLEEP = 1000;
            const int RETRIES = 3;

            var retriesLeft = RETRIES;
            Size currentSize;
            do
            {
                logger.Verbose("Trying to set the browser size to: {0}", requiredSize);
                setBrowserSize(driver, requiredSize);
                Thread.Sleep(SLEEP);
                currentSize = getBrowserSize(driver);
                logger.Verbose("Current browser size: {0}", currentSize);
            }
            while (--retriesLeft > 0 && currentSize != requiredSize);

            return currentSize == requiredSize;
        }

        private static Size SetViewportSizeAttempt1<IWebDriver>(
            Logger logger, IWebDriver driver, Size requiredSize, Size actualViewportSize,
            Action<IWebDriver> moveBrowserToOrigin,
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
        {
            // Move the window to (0,0) to have the best chance to be able to
            // set the viewport size as requested.
            if (moveWindow_)
            {
                moveBrowserToOrigin(driver);
            }

            SetBrowserSizeByViewportSize(logger, driver, actualViewportSize, requiredSize, setBrowserSize, getBrowserSize);

            actualViewportSize = GetViewportSize(logger, driver);

            logger.Verbose("Current viewport size: {0}", actualViewportSize);
            return actualViewportSize;
        }

        private static Size SetViewportSizeAttempt2<IWebDriver>(
            Logger logger, IWebDriver driver, Size requiredSize, Size actualViewportSize,
            Action<IWebDriver, Size> setBrowserSize,
            Func<IWebDriver, Size> getBrowserSize)
        {
            // Additional attempt. This Solves the "maximized browser" bug
            // (border size for maximized browser sometimes different than
            // non-maximized, so the original browser size calculation is
            // wrong).
            logger.Verbose("Trying workaround for maximization...");
            SetBrowserSizeByViewportSize(logger, driver, actualViewportSize, requiredSize, setBrowserSize, getBrowserSize);

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
        public static string SetOverflow(string overflow, IEyesJsExecutor jsExecutor, object rootElement)
        {
            ArgumentGuard.NotNull(jsExecutor, nameof(jsExecutor));
            ArgumentGuard.NotNull(rootElement, nameof(rootElement));

            var result = jsExecutor.ExecuteScript(JSSetOverflow_.Fmt(overflow), rootElement);
            Thread.Sleep(SetOverflowWaitMS_);
            return result as string;
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

        public static IWebElement GetDefaultRootElement<IWebDriver, IWebElement>(
            IWebDriver driver, GetHtmlAndBodyElementsDelegate<IWebDriver, IWebElement> getHtmlAndBodyElements,
            Func<IWebElement, string> getTagName, Logger logger = null) where IWebElement : class
        {
            IWebElement chosenElement;
            IEyesJsExecutor jsExecutor = (IEyesJsExecutor)driver;

            IWebElement scrollingElement;
            try
            {
                scrollingElement = (IWebElement)jsExecutor.ExecuteScript("return document.scrollingElement");
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger, Stage.Check, e);
                scrollingElement = null;
            }

            getHtmlAndBodyElements(driver, out IWebElement html, out ReadOnlyCollection<IWebElement> bodies);

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
            logger?.Verbose("Chosen default root element is {0}", getTagName(chosenElement));
            return chosenElement;
        }

        internal delegate void GetHtmlAndBodyElementsDelegate<IWebDriver, IWebElement>(IWebDriver driver, out IWebElement html, out ReadOnlyCollection<IWebElement> bodies);

        internal static bool CanScrollVertically(IEyesJsExecutor jsExecutor, object element)
        {
            return (bool)jsExecutor.ExecuteScript("return arguments[0].scrollHeight > arguments[0].clientHeight", element);
        }

        public static Size GetScrollSize(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (jsExecutor.ExecuteScript(JS_GET_SCROLL_SIZE, element) is string scrollSizeStr)
            {
                string[] data = scrollSizeStr.Split(',');
                return new Size(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Size.Empty;
        }

        public static Rectangle GetClientBounds(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Rectangle GetClientBoundsWithoutBorders(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT_WITHOUT_BORDERS, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Point GetOffsetPosition(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (jsExecutor.ExecuteScript(JS_GET_OFFSET_POSITION, element) is string scrollPositionStr)
            {
                string[] data = scrollPositionStr.Split(',');
                return new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Point.Empty;
        }

        public static Point GetScrollPosition(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (jsExecutor.ExecuteScript(JS_GET_SCROLL_POSITION, element) is string scrollPositionStr)
            {
                string[] data = scrollPositionStr.Split(',');
                return new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Point.Empty;
        }

        public static string GetComputedStyle(string propertyName, object element, IEyesJsExecutor jsExecutor)
        {
            string scriptToExec = string.Format(JS_GET_COMPUTED_STYLE_FORMATTED_STR, propertyName);
            return (string)jsExecutor.ExecuteScript(scriptToExec, element);
        }

        public static Point GetClientVisualOffset(object element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_CLIENT_VISUAL_OFFSET, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            return new Point(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
        }

    }
}
