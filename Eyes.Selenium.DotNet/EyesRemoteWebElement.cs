using Applitools.Utils;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium
{
    public class EyesRemoteWebElement : IWebElement, IEquatable<EyesRemoteWebElement>, IEquatable<IWebElement>
    {
        #region Fields

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

        private readonly string JS_GET_SCROLL_LEFT = "return arguments[0].scrollLeft;";
        private readonly string JS_GET_SCROLL_TOP = "return arguments[0].scrollTop;";
        private readonly string JS_GET_SCROLL_WIDTH = "return arguments[0].scrollWidth;";
        private readonly string JS_GET_SCROLL_HEIGHT = "return arguments[0].scrollHeight;";

        private static readonly string JS_GET_SCROLL_POSITION = "return arguments[0].scrollLeft + ',' + arguments[0].scrollTop;";
        private static readonly string JS_GET_SCROLL_SIZE = "return arguments[0].scrollWidth + ',' + arguments[0].scrollHeight;";
        private static readonly string JS_GET_OFFSET_POSITION = "return arguments[0].offsetLeft + ',' + arguments[0].offsetTop;";
        private readonly string JS_GET_CLIENT_WIDTH = "return arguments[0].clientWidth;";
        private readonly string JS_GET_CLIENT_HEIGHT = "return arguments[0].clientHeight;";

        private static readonly string JS_GET_CLIENT_SIZE = "return arguments[0].clientWidth + ',' + arguments[0].clientHeight;";

        private const string JS_GET_ENTIRE_ELEMENT_SIZE =
            "var width = Math.max(arguments[0].clientWidth, arguments[0].scrollWidth);" +
            "var height = Math.max(arguments[0].clientHeight, arguments[0].scrollHeight);" +
            "return (width + ';' + height);";

        private readonly string JS_SCROLL_TO_FORMATTED_STR =
            "arguments[0].scrollLeft = {0};" +
            "arguments[0].scrollTop = {1};";

        //private readonly string JS_SET_OVERFLOW_FORMATTED_STR = "var orig = arguments[0].style.overflow; arguments[0].style.overflow = '{0}'; return orig;";
        private readonly string JS_GET_OVERFLOW = "return arguments[0].style.overflow;";

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

        private EyesWebDriver eyesDriver_;
        private IWebElement webElement_;

        #endregion

        #region Constructors

        public EyesRemoteWebElement(Logger logger, EyesWebDriver eyesDriver, IWebElement webElement)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(eyesDriver, nameof(eyesDriver));
            ArgumentGuard.NotNull(webElement, nameof(webElement));

            Logger = logger;
            eyesDriver_ = eyesDriver;
            webElement_ = webElement;

            //if (webElement_ == null)
            //{
            //    string errMsg = "The input web element is not a RemoteWebElement ({0})"
            //        .Fmt(webElement.GetType().Name);
            //    Logger.Log(TraceLevel.Error, Stage.General, new { errMsg });
            //    throw new EyesException(errMsg);
            //}

            PropertyInfo pi = typeof(RemoteWebElement).GetProperty("Id", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            Id = (string)pi.GetValue(webElement);
            TagName = webElement.TagName;
        }

        public string TagName { get; private set; }
        public string Id { get; private set; }

        #endregion

        #region Properties

        public IWebElement WrappedElement
        {
            get { return webElement_; }
        }

        protected Logger Logger { get; private set; }

        /* public new Point Location
         {
             get
             {
                 string elementOffsetPosition = (string)eyesDriver_.ExecuteScript(JS_GET_OFFSET_POSITON, this);
                 string[] leftAndTop = elementOffsetPosition.Split(',');
                 int x = (int)Math.Ceiling(Convert.ToDouble(leftAndTop[0]));
                 int y = (int)Math.Ceiling(Convert.ToDouble(leftAndTop[1]));
                 return new Point(x, y);
             }
         }
         */

        /*  public Point Location
          {
              get
              {
                  Logger.Verbose(this.ToString());
                  Dictionary<string, object> dictionary = new Dictionary<string, object>() { { "id", Id } };
                  Response response;
                  try
                  {
                      response = Execute(DriverCommand.GetElementLocation, dictionary);
                  }
                  catch (NullReferenceException)
                  {
                      response = Execute(DriverCommand.GetElementRect, dictionary);
                  }

                  try
                  {
                      Dictionary<string, object> obj = (Dictionary<string, object>)response.Value;

                      int x = (int)Math.Round(Convert.ToDouble(obj["x"], CultureInfo.InvariantCulture));
                      int y = (int)Math.Round(Convert.ToDouble(obj["y"], CultureInfo.InvariantCulture));
                      return new Point(x, y);
                  }
                  catch
                  {
                      Logger.Log(TraceLevel.Error, Stage.General, 
                          new { errMsg = "error parsing response: " + response.Value });
                      throw;
                  }
              }
          }*/

        public Point Location => webElement_.Location;

        public int ScrollLeft
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_LEFT, this)); }
        }

        public int ScrollTop
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_TOP, this)); }
        }

        public Point ScrollPosition => GetScrollPosition(webElement_, eyesDriver_);

        public static Point GetScrollPosition(IWebElement targetElement, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (jsExecutor.ExecuteScript(JS_GET_SCROLL_POSITION, targetElement) is string scrollPositionStr)
            {
                string[] data = scrollPositionStr.Split(',');
                return new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Point.Empty;
        }

        public int ScrollWidth
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_WIDTH, this)); }
        }

        public int ScrollHeight
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_HEIGHT, this)); }
        }

        public Size ScrollSize => GetScrollSize(webElement_, eyesDriver_, Logger);

        public static Size GetScrollSize(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (jsExecutor.ExecuteScript(JS_GET_SCROLL_SIZE, element) is string scrollSizeStr)
            {
                string[] data = scrollSizeStr.Split(',');
                return new Size(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Size.Empty;
        }

        public int ClientWidth
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_CLIENT_WIDTH, this)); }
        }

        public int ClientHeight
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_CLIENT_HEIGHT, this)); }
        }

        public Size ClientSize => GetClientSize(webElement_, eyesDriver_, Logger);

        public static Size GetClientSize(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            string sizeStr = jsExecutor.ExecuteScript(JS_GET_CLIENT_SIZE, element) as string;
            if (sizeStr == null) { return Size.Empty; }
            sizeStr = sizeStr.Replace("px", "");
            string[] data = sizeStr.Split(',');
            return new Size(
                (int)Math.Ceiling(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                (int)Math.Ceiling(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
        }

        public static Size GetEntireSize(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            logger.Verbose("enter (element: {0})", element);
            string entireSizeStr = (string)jsExecutor.ExecuteScript(JS_GET_ENTIRE_ELEMENT_SIZE, element);
            string[] wh = entireSizeStr.Split(';');
            Size size = new Size(Convert.ToInt32(wh[0]), Convert.ToInt32(wh[1]));
            logger.Verbose(size.ToString());
            return size;
        }

        /// <summary>
        /// Gets or sets the overflow of the element.
        /// </summary>
        public string Overflow
        {
            get
            {
                return eyesDriver_.ExecuteScript(JS_GET_OVERFLOW, webElement_).ToString();
            }
            set
            {
                EyesSeleniumUtils.SetOverflow(value, eyesDriver_, webElement_);
            }
        }

        public string SetOverflow(string value)
        {
            return EyesSeleniumUtils.SetOverflow(value, eyesDriver_, webElement_);
        }

        public RectangularMargins BorderWidths => GetBorderWidths(webElement_, eyesDriver_, Logger);

        public static RectangularMargins GetBorderWidths(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            try
            {
                string bordersStr = jsExecutor.ExecuteScript(JS_GET_BORDER_WIDTHS, element) as string;
                if (bordersStr == null) { return new RectangularMargins(); }
                bordersStr = bordersStr.Replace("px", "");
                string[] data = bordersStr.Split(',');
                return new RectangularMargins(
                    (int)Math.Ceiling(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                    (int)Math.Ceiling(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                    (int)Math.Ceiling(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)),
                    (int)Math.Ceiling(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo))
                );
            }
            catch
            {
                return new RectangularMargins(
                    GetPixelComputedStyle("border-left-width", element, jsExecutor),
                    GetPixelComputedStyle("border-top-width", element, jsExecutor),
                    GetPixelComputedStyle("border-right-width", element, jsExecutor),
                    GetPixelComputedStyle("border-bottom-width", element, jsExecutor)
                );
            }
        }

        public SizeAndBorders SizeAndBorders => GetSizeAndBorders(webElement_, eyesDriver_, Logger);

        string IWebElement.TagName => webElement_.TagName;

        public string Text => webElement_.Text;

        public bool Enabled => webElement_.Enabled;

        public bool Selected => webElement_.Selected;

        public Size Size => webElement_.Size;

        public bool Displayed => webElement_.Displayed;

        public static SizeAndBorders GetSizeAndBorders(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            SizeAndBorders retval = new SizeAndBorders();
            try
            {
                if (jsExecutor.ExecuteScript(JS_GET_SIZE_AND_BORDER_WIDTHS, element) is string sizeAndBordersStr)
                {
                    sizeAndBordersStr = sizeAndBordersStr.Replace("px", "");
                    string[] data = sizeAndBordersStr.Split(',');
                    retval.Size = new Size(
                        (int)Math.Ceiling(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                        (int)Math.Ceiling(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo))
                    );
                    retval.Borders = new RectangularMargins(
                        (int)Math.Ceiling(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)),
                        (int)Math.Ceiling(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)),
                        (int)Math.Ceiling(Convert.ToSingle(data[4], NumberFormatInfo.InvariantInfo)),
                        (int)Math.Ceiling(Convert.ToSingle(data[5], NumberFormatInfo.InvariantInfo))
                    );
                }
            }
            catch
            {
                retval.Size = GetClientSize(element, jsExecutor, logger);
                retval.Borders = GetBorderWidths(element, jsExecutor, logger);
            }
            return retval;
        }

        #endregion

        #region Methods

        public Point ScrollTo(Point location)
        {
            if (eyesDriver_.ExecuteScript(string.Format(JS_SCROLL_TO_FORMATTED_STR, location.X, location.Y) + JS_GET_SCROLL_POSITION, this) is string scrollPositionStr)
            {
                string[] data = scrollPositionStr.Split(',');
                return new Point(
                    (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                    (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
            }
            return Point.Empty;
        }

        public void SendKeys(string text)
        {
            eyesDriver_.UserActionsEyes.AddKeyboardTrigger(webElement_, text);
            webElement_.SendKeys(text);
        }

        public static Point GetOffsetPosition(IWebElement targetElement, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (targetElement is EyesRemoteWebElement erwe) { targetElement = erwe.WrappedElement; }
            if (jsExecutor.ExecuteScript(JS_GET_OFFSET_POSITION, targetElement) is string scrollPositionStr)
            {
                string[] data = scrollPositionStr.Split(',');
                return new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
            }
            return Point.Empty;
        }

        public static Point GetOffsetPositionFromActiveFrame(IWebElement targetElement, EyesWebDriver driver, Logger logger)
        {
            if (targetElement is EyesRemoteWebElement teerwe) { targetElement = teerwe.WrappedElement; }
            IWebElement activeFrame = driver.FindElement(By.TagName("html"));
            if (activeFrame is EyesRemoteWebElement aferwe) { activeFrame = aferwe.WrappedElement; }
            Point afp = GetScrollPosition(activeFrame, driver, logger);
            Point elp = GetOffsetPosition(targetElement, driver, logger);
            elp.Offset(afp);
            return elp;
        }

        public Region GetBounds()
        {
            Point weLocation = webElement_.Location;
            int left = weLocation.X;
            int top = weLocation.Y;
            int width = 0;
            int height = 0;

            try
            {
                // Width cannot be extracted on all platforms.
                Size weSize = webElement_.Size;
                width = weSize.Width;
                height = weSize.Height;
            }
            catch
            {
            }

            if (left < 0)
            {
                width = Math.Max(0, width + left);
                left = 0;
            }

            if (top < 0)
            {
                height = Math.Max(0, height + top);
                top = 0;
            }

            return new Region(left, top, width, height, CoordinatesTypeEnum.CONTEXT_RELATIVE);
        }

        public Rectangle GetClientBounds() => GetClientBounds(webElement_, eyesDriver_, Logger);

        public static Rectangle GetClientBounds(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (element is EyesRemoteWebElement erwe) { element = erwe.WrappedElement; }
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Rectangle GetClientBoundsWithoutBorders(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (element is EyesRemoteWebElement erwe) { element = erwe.WrappedElement; }
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT_WITHOUT_BORDERS, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Point GetClientVisualOffset(IWebElement element, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (element is EyesRemoteWebElement erwe) { element = erwe.WrappedElement; }
            string result = (string)jsExecutor.ExecuteScript(JS_GET_CLIENT_VISUAL_OFFSET, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            return new Point(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
        }

        public Rectangle GetVisibleElementRect() => GetVisibleElementRect(webElement_, eyesDriver_, Logger);

        public static Rectangle GetVisibleElementRect(IWebElement webElement, IEyesJsExecutor jsExecutor, Logger logger = null)
        {
            if (webElement is EyesRemoteWebElement erwe) { webElement = erwe.WrappedElement; }
            string result = (string)jsExecutor.ExecuteScript(JS_GET_VISIBLE_ELEMENT_RECT, webElement);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public void Click()
        {
            // Letting the driver know about the current action.
            int left = webElement_.Location.X;
            int top = webElement_.Location.Y;
            int width = 0;
            int height = 0;

            try
            {
                // Width cannot be extracted on all platforms so computing the offset may fail.
                width = webElement_.Size.Width;
                height = webElement_.Size.Height;
            }
            catch
            {
            }

            if (left < 0)
            {
                width = Math.Max(0, width + left);
                left = 0;
            }

            if (top < 0)
            {
                height = Math.Max(0, height + top);
                top = 0;
            }

            int offsetX = width / 2;
            int offsetY = height / 2;

            var control = new Rectangle(left, top, width, height);
            var offset = new Point(offsetX, offsetY);
            eyesDriver_.UserActionsEyes.AddMouseTrigger(MouseAction.Click, webElement_, offset);

            Logger.Verbose("Click({0} {1})", control, offset);

            webElement_.Click();
        }

        public void Clear()
        {
            webElement_.Clear();
        }

        public string GetAttribute(string attributeName)
        {
            return webElement_.GetAttribute(attributeName);
        }

        public string GetComputedStyle(string propertyName) => GetComputedStyle(propertyName, webElement_, eyesDriver_);

        public static string GetComputedStyle(string propertyName, IWebElement element, IEyesJsExecutor jsExecutor)
        {
            string scriptToExec = string.Format(JS_GET_COMPUTED_STYLE_FORMATTED_STR, propertyName);
            return (string)jsExecutor.ExecuteScript(scriptToExec, element);
        }

        /// <summary>
        /// Gets the integer value of a computed style.
        /// </summary>
        /// <param name="propStyle">The style's property name.</param>
        /// <returns>The integer value of a computed style.</returns>
        public int GetPixelComputedStyle(string propStyle) => GetPixelComputedStyle(propStyle, webElement_, eyesDriver_);

        public static int GetPixelComputedStyle(string propStyle, IWebElement element, IEyesJsExecutor jsExecutor)
        {
            string computedStyle = GetComputedStyle(propStyle, element, jsExecutor).Trim().Replace("px", "");
            return (int)Math.Ceiling(double.Parse(computedStyle));
        }

        public override string ToString()
        {
#if DEBUG
            try
            {
                return $"EyesRemoteWebElement: {TagName} (id = {Id})";
            }
            catch
            {
                return $"EyesRemoteWebElement: (id = {Id})";
            }
#else
            return $"EyesRemoteWebElement: (id = {Id})";
#endif
        }

        /// <summary>
        /// For RemoteWebElement object, the function returns an 
        /// EyesRemoteWebElement object. For all other types of WebElement, 
        /// the function returns the original object.
        /// </summary>
        private EyesRemoteWebElement WrapElementIfRemote_(IWebElement elementToWrap)
        {
            return new EyesRemoteWebElement(Logger, eyesDriver_, elementToWrap);
        }

        /// <summary>
        /// For RemoteWebElement object, the function returns an 
        /// EyesRemoteWebElement object. For all other types of WebElement, 
        /// the function returns the original object.
        /// </summary>
        /// <param name="elementsToWrap"></param>
        /// <returns></returns>
        private IList<IWebElement> WrapElements_(IEnumerable<IWebElement> elementsToWrap)
        {
            // This list will contain the found elements wrapped with our class.
            List<IWebElement> wrappedElementsList = new List<IWebElement>();

            // TODO - Daniel, Support additional implementation of web element
            foreach (IWebElement element in elementsToWrap)
            {
                wrappedElementsList.Add(WrapElementIfRemote_(element));
            }

            return wrappedElementsList;
        }

        public bool Equals(EyesRemoteWebElement other)
        {
            return this.Id == other.Id;
        }

        public bool Equals(IWebElement other)
        {
            if (other is EyesRemoteWebElement erwe)
            {
                return this.Equals(erwe);
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is EyesRemoteWebElement erwe)
            {
                return this.Equals(erwe);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void Submit()
        {
            webElement_.Submit();
        }

        public string GetProperty(string propertyName)
        {
            return webElement_.GetProperty(propertyName);
        }

        public string GetCssValue(string propertyName)
        {
            return webElement_.GetCssValue(propertyName);
        }

        public IWebElement FindElement(By by)
        {
            return webElement_.FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return webElement_.FindElements(by);
        }

        #endregion
    }
}
