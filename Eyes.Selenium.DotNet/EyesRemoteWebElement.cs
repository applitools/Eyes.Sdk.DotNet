using Applitools.Utils;
using Applitools.Utils.Geometry;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium
{
    public class EyesRemoteWebElement : RemoteWebElement,
        IWebElement, ISearchContext, IFindsByLinkText, IFindsById, IFindsByName,
        IFindsByTagName, IFindsByClassName, IFindsByXPath, IFindsByPartialLinkText,
        IFindsByCssSelector, IWrapsDriver, ILocatable, IWrapsElement,
        IEquatable<EyesRemoteWebElement>, IEquatable<IWebElement>
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

        private readonly string JS_GET_SCROLL_POSITION = "return arguments[0].scrollLeft + ',' + arguments[0].scrollTop;";
        private static readonly string JS_GET_SCROLL_SIZE = "return arguments[0].scrollWidth + ',' + arguments[0].scrollHeight;";

        private readonly string JS_GET_CLIENT_WIDTH = "return arguments[0].clientWidth;";
        private readonly string JS_GET_CLIENT_HEIGHT = "return arguments[0].clientHeight;";

        private static readonly string JS_GET_CLIENT_SIZE = "return arguments[0].clientWidth + ',' + arguments[0].clientHeight;";

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
      x: bcr.left + el.clientLeft,
      y: bcr.top + el.clientTop,
      width: el.clientWidth,
      height: el.clientHeight,
    }
    if (el.tagName === 'IFRAME'){
      intersected.x += cr.x
      intersected.y += cr.y
    } 
    
    if (el.tagName !== 'BODY') { // The body element behavior is special... so to say, so we don't want to account it.
      intersected = intersect(cr, intersected)
    }

    el = el.parentElement;
    if (el != null && el.tagName === 'HTML'){
      el = el.ownerDocument.defaultView.frameElement
    }
  }
  return intersected.x+';'+intersected.y+';'+intersected.width+';'+intersected.height;
  function intersect(rect1, rect2) {
    var intersectionLeft = rect1.x >= rect2.x ? rect1.x : rect2.x
    var intersectionTop = rect1.y >= rect2.y ? rect1.y : rect2.y
    var rect1Right = rect1.x + rect1.width
    var rect2Right = rect2.x + rect2.width
    var intersectionRight = rect1Right <= rect2Right ? rect1Right : rect2Right
    var intersectionWidth = intersectionRight - intersectionLeft
    var rect1Bottom = rect1.y + rect1.height
    var rect2Bottom = rect2.y + rect2.height
    var intersectionBottom = rect1Bottom <= rect2Bottom ? rect1Bottom : rect2Bottom
    var intersectionHeight = intersectionBottom - intersectionTop
    return {
      x: intersectionLeft,
      y: intersectionTop,
      width: intersectionWidth,
      height: intersectionHeight,
    }
  }
}
return getVisibleElementRect(arguments[0])";

        private EyesWebDriver eyesDriver_;
        private RemoteWebElement webElement_;

        #endregion

        #region Constructors

        public EyesRemoteWebElement(Logger logger, EyesWebDriver eyesDriver, IWebElement webElement)
            : base(eyesDriver.RemoteWebDriver, null)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(eyesDriver, nameof(eyesDriver));
            ArgumentGuard.NotNull(webElement, nameof(webElement));

            Logger = logger;
            eyesDriver_ = eyesDriver;
            webElement_ = webElement as RemoteWebElement;

            if (webElement_ == null)
            {
                string errMsg = "The input web element is not a RemoteWebElement ({0})"
                    .Fmt(webElement.GetType().Name);
                Logger.Log(errMsg);
                throw new EyesException(errMsg);
            }

            FieldInfo fi = typeof(RemoteWebElement).GetField("elementId", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(this, fi.GetValue(webElement));
        }

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

        public new Point Location
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
                    Logger.Log("error parsing response: " + response.Value);
                    throw;
                }
            }
        }

        public int ScrollLeft
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_LEFT, this)); }
        }

        public int ScrollTop
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_TOP, this)); }
        }

        public Point ScrollPosition
        {
            get
            {
                if (eyesDriver_.ExecuteScript(JS_GET_SCROLL_POSITION, this) is string scrollPositionStr)
                {
                    string[] data = scrollPositionStr.Split(',');
                    return new Point(Convert.ToInt32(data[0]), Convert.ToInt32(data[1]));
                }
                return Point.Empty;
            }
        }

        public int ScrollWidth
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_WIDTH, this)); }
        }

        public int ScrollHeight
        {
            get { return Convert.ToInt32(eyesDriver_.ExecuteScript(JS_GET_SCROLL_HEIGHT, this)); }
        }

        public Size ScrollSize => GetScrollSize(this, eyesDriver_, Logger);

        public static Size GetScrollSize(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
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

        public Size ClientSize => GetClientSize(this, eyesDriver_, Logger);

        public static Size GetClientSize(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
        {
            string sizeStr = jsExecutor.ExecuteScript(JS_GET_CLIENT_SIZE, element) as string;
            if (sizeStr == null) { return Size.Empty; }
            sizeStr = sizeStr.Replace("px", "");
            string[] data = sizeStr.Split(',');
            return new Size(
                (int)Math.Ceiling(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                (int)Math.Ceiling(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
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
                EyesSeleniumUtils.SetOverflow(value, eyesDriver_, this);
            }
        }

        public string SetOverflow(string value)
        {
            return EyesSeleniumUtils.SetOverflow(value, eyesDriver_, this);
        }

        public RectangularMargins BorderWidths => GetBorderWidths(this, eyesDriver_, Logger);

        public static RectangularMargins GetBorderWidths(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
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

        public SizeAndBorders SizeAndBorders => GetSizeAndBorders(this, eyesDriver_, Logger);

        public static SizeAndBorders GetSizeAndBorders(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
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

        private static IDictionary<string, IPositionProvider> positionProviderById_ = new ConcurrentDictionary<string, IPositionProvider>();
        public IPositionProvider PositionProvider
        {
            get
            {
                positionProviderById_.TryGetValue(IdForDictionary, out IPositionProvider positionProvider);
                Logger.Verbose("Getting position provider for element Id {0}: {1}", IdForDictionary, positionProvider);
                return positionProvider;
            }
            internal set
            {
                Logger.Verbose("Setting position provider for element Id {0}: {1}", IdForDictionary, value);
                positionProviderById_[IdForDictionary] = value;
            }
        }

        internal string Id_ => Id;
        public string IdForDictionary => Id + "_" + eyesDriver_.RemoteWebDriver.SessionId; // Safari browser uses simple element ids so I chain Selenium session id

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

        public new void SendKeys(string text)
        {
            var control = GetBounds();
            eyesDriver_.Eyes?.AddKeyboardTrigger(control.ToRectangle(), text);

            webElement_.SendKeys(text);
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

        public static Rectangle GetClientBounds(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Rectangle GetClientBoundsWithoutBorders(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_BOUNDING_CLIENT_RECT_WITHOUT_BORDERS, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public static Point GetClientVisualOffset(IWebElement element, IJavaScriptExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_CLIENT_VISUAL_OFFSET, element);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            return new Point(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)));
        }

        public Rectangle GetVisibleElementRect() => GetVisibleElementRect(webElement_, eyesDriver_, Logger);
        
        public static Rectangle GetVisibleElementRect(IWebElement webElement, IJavaScriptExecutor jsExecutor, Logger logger = null)
        {
            string result = (string)jsExecutor.ExecuteScript(JS_GET_VISIBLE_ELEMENT_RECT, webElement);
            logger?.Verbose(result);
            string[] data = result.Split(';');
            Rectangle rect = new Rectangle(
                (int)Math.Round(Convert.ToSingle(data[0], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[1], NumberFormatInfo.InvariantInfo)),
                (int)Math.Round(Convert.ToSingle(data[2], NumberFormatInfo.InvariantInfo)), (int)Math.Round(Convert.ToSingle(data[3], NumberFormatInfo.InvariantInfo)));
            return rect;
        }

        public new void Click()
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
            eyesDriver_.Eyes?.AddMouseTrigger(MouseAction.Click, control, offset);

            Logger.Verbose("Click({0} {1})", control, offset);

            webElement_.Click();
        }

        public new void Clear()
        {
            webElement_.Clear();
        }

        public new string GetAttribute(string attributeName)
        {
            return webElement_.GetAttribute(attributeName);
        }

        public string GetComputedStyle(string propertyName) => GetComputedStyle(propertyName, webElement_, eyesDriver_);

        public static string GetComputedStyle(string propertyName, IWebElement element, IJavaScriptExecutor jsExecutor)
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

        public static int GetPixelComputedStyle(string propStyle, IWebElement element, IJavaScriptExecutor jsExecutor)
        {
            string computedStyle = GetComputedStyle(propStyle, element, jsExecutor).Trim().Replace("px", "");
            return (int)Math.Ceiling(double.Parse(computedStyle));
        }

        public new IWebElement FindElement(By by)
        {
            return WrapElementIfRemote_(webElement_.FindElement(by));
        }

        public new ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElements(by)));
        }

        public new IWebElement FindElementById(string id)
        {
            return WrapElementIfRemote_(webElement_.FindElementById(id));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsById(id)));
        }

        public new IWebElement FindElementByLinkText(string linkText)
        {
            return WrapElementIfRemote_(webElement_.FindElementByLinkText(linkText));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByLinkText(linkText)));
        }

        public new IWebElement FindElementByName(string name)
        {
            return WrapElementIfRemote_(webElement_.FindElementByName(name));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByName(name)));
        }

        public new IWebElement FindElementByTagName(string tagName)
        {
            return WrapElementIfRemote_(webElement_.FindElementByTagName(tagName));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByTagName(tagName)));
        }

        public new IWebElement FindElementByClassName(string className)
        {
            return WrapElementIfRemote_(webElement_.FindElementByClassName(className));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByClassName(className)));
        }

        public new IWebElement FindElementByXPath(string xpath)
        {
            return WrapElementIfRemote_(webElement_.FindElementByXPath(xpath));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByXPath(xpath)));
        }

        public new IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            return WrapElementIfRemote_(webElement_.FindElementByPartialLinkText(partialLinkText));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByPartialLinkText(string partialLinkText)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByPartialLinkText(partialLinkText)));
        }

        public new IWebElement FindElementByCssSelector(string cssSelector)
        {
            return WrapElementIfRemote_(webElement_.FindElementByCssSelector(cssSelector));
        }

        public new ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return new ReadOnlyCollection<IWebElement>(
                WrapElements_(webElement_.FindElementsByCssSelector(cssSelector)));
        }

        public override string ToString()
        {
#if DEBUG
            try
            {
                return $"EyesRemoteWebElement: {base.TagName} (id = {base.Id})";
            }
            catch
            {
                return $"EyesRemoteWebElement: (id = {base.Id})";
            }
#else
            return $"EyesRemoteWebElement: (id = {base.Id})";
#endif
        }

        /// <summary>
        /// For RemoteWebElement object, the function returns an 
        /// EyesRemoteWebElement object. For all other types of WebElement, 
        /// the function returns the original object.
        /// </summary>
        private IWebElement WrapElementIfRemote_(IWebElement elementToWrap)
        {
            if (elementToWrap is RemoteWebElement remoteWebElement)
            {
                return new EyesRemoteWebElement(Logger, eyesDriver_, elementToWrap);
            }

            return elementToWrap;
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
            return base.GetHashCode();
        }
        #endregion
    }
}
