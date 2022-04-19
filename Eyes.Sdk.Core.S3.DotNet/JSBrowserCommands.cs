namespace Applitools.Utils
{
    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Threading;

    public static class JSBrowserCommands
    {

        #region Fields

        private const string RETURN_PLACEHOLDER_ = "##RETURN_HOLDER##";
        private const string JSGetViewportHeight_ =
            RETURN_PLACEHOLDER_ + "(function() {" +
            "var height = undefined;" +
            "  if (window.innerHeight) {height = window.innerHeight;}" +
            "  else if (document.documentElement && document.documentElement.clientHeight)" +
            " {height = document.documentElement.clientHeight;}" +
            "  else { var b = document.getElementsByTagName('body')[0]; if (b.clientHeight)" +
            " {height = b.clientHeight;}} " +
            "return (height + '');" +
            "}());";

        private const string JSGetViewportWidth_ =
            RETURN_PLACEHOLDER_ + "(function() {" +
            "var width = undefined;" +
            " if (window.innerWidth) {width = window.innerWidth;}" +
            " else if (document.documentElement && document.documentElement.clientWidth)" +
            " {width = document.documentElement.clientWidth;}" +
            " else { var b = document.getElementsByTagName('body')[0]; if (b.clientWidth)" +
            " {width = b.clientWidth;}} " +
            "return (width + '');" +
            "}());";

        private const string JSGetScrollPosition_ =
            RETURN_PLACEHOLDER_ + "(function() {" +
            "var xo = window.scrollX;" +
            "if (xo !== 0 && !xo) {" +
            "var doc = document.documentElement; var left = (window.pageXOffset || doc.scrollLeft) - (doc.clientLeft || 0);" +
            "xo = left;}" +
            "var yo = window.scrollY;" +
            "if (yo !== 0 && !yo) {" +
            "var doc = document.documentElement; var top = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);" +
            "yo = top;} " +
            "return (xo + ',' + yo);" +
            "}());";

        private const string JSGetEntireElementSize_ =
           RETURN_PLACEHOLDER_ + "(function() {" +
            "var width = Math.max(arguments[0].clientWidth, arguments[0].scrollWidth);" +
            "var height = Math.max(arguments[0].clientHeight, arguments[0].scrollHeight);" +
            "return (width+';'+height);" +
            "}());";

        // IMPORTANT: Notice there's a major difference between scrollWidth
        // and scrollHeight. While scrollWidth is the maximum between an
        // element's width and its content width, scrollHeight might be
        // smaller (!) than the clientHeight, which is why we take the
        // maximum between them.

        private const string JSGetEntirePageSize_ =
            RETURN_PLACEHOLDER_ + "(function() {" +
            "var width = Math.max(document.documentElement.scrollWidth, document.body.scrollWidth);" +
            "var height = Math.max(document.documentElement.clientHeight, document.body.clientHeight," +
            " document.documentElement.scrollHeight, document.body.scrollHeight);" +
            "return (width + ',' + height);" +
            "}());";

        private const string JSGetCurrentTransform_ =
            RETURN_PLACEHOLDER_ + " document.documentElement.style.transform;";

        private const string JSSetTransform_ =
            RETURN_PLACEHOLDER_ + " (function() {" +
            "var originalTransform = document.documentElement.style.transform;" +
            "document.documentElement.style.transform = '{0}';" +
            "return originalTransform;" +
            "}());";

        private const string JSSelectRootElement_ = RETURN_PLACEHOLDER_ + " (function() {" +
            "var docElemScrollHeightBefore = document.documentElement.scrollHeight; " +
            "var originalBodyOverflow = document.body.style.overflow; " +
            "document.body.style.overflow = 'hidden'; " +
            "var docElemScrollHeightAfter = document.documentElement.scrollHeight; " +
            "if (docElemScrollHeightBefore != docElemScrollHeightAfter) " +
                "var retval = 'documentElement'; " +
            "else " +
                "var retval = 'body'; " +
            "document.body.style.overflow = originalBodyOverflow; " +
            "return retval;" +
            "}());";

        private const string JSSetOverflow_ =
            RETURN_PLACEHOLDER_ + " (function() {" +
            "var origOF = document.{1}.style.overflow;" +
            "document.{1}.style.overflow = '{0}';" +
            "return origOF;" +
            "}());";

        private const string JSSetBodyOverflow_ =
            RETURN_PLACEHOLDER_ + " (function() {" +
            "var origOF = document.body.style.overflow;" +
            "document.body.style.overflow = '{0}';" +
            "return origOF;" +
            "}());";

        private const string JSSetDocElementOverflow_ =
            RETURN_PLACEHOLDER_ + " (function() {" +
            "var origOF = document.documentElement.style.overflow;" +
            "document.documentElement.style.overflow = '{0}';" +
            "return origOF;" +
            "}());";

        private const int SetOverflowWaitMS_ = 200;

        private const string JSComputeContentEntireSize_ =
            "var scrollWidth = document.documentElement.scrollWidth; " +
                    "var bodyScrollWidth = document.body.scrollWidth; " +
                    "var totalWidth = Math.max(scrollWidth, bodyScrollWidth); " +
                    "var clientHeight = document.documentElement.clientHeight; " +
                    "var bodyClientHeight = document.body.clientHeight; " +
                    "var scrollHeight = document.documentElement.scrollHeight; " +
                    "var bodyScrollHeight = document.body.scrollHeight; " +
                    "var maxDocElementHeight = Math.max(clientHeight, scrollHeight); " +
                    "var maxBodyHeight = Math.max(bodyClientHeight, bodyScrollHeight); " +
                    "var totalHeight = Math.max(maxDocElementHeight, maxBodyHeight); ";

        private const string JSReturnContentEntireSize_ = JSComputeContentEntireSize_ + "return (totalWidth + ',' + totalHeight);";
        private const string JSScrollToBottomRight_ = JSComputeContentEntireSize_ + "window.scrollTo(totalWidth, totalHeight);";

        #endregion Fields

        #region Types

        /// <summary>
        /// The "WithReturn" does NOT(!) refer to whether or not a value is returned. Instead, it 
        /// refers to whether or not a "return" command should explicitly be written when 
        /// returning a value (some implementations, such as LeanFT, require you to just write 
        /// the value to be returned as the last command and explicitly omit the "return" keyword).
        /// </summary>
        public static class WithReturn
        {
            private const string RETURN_STRING = "return";
            /// <summary>
            /// Get the scroll position of the current frame.
            /// </summary>
            /// <returns>The scroll position of the current frame.</returns>
            public static Point GetCurrentScrollPosition(IEyesJsExecutor executor)
            {
                return GetCurrentScrollPosition_(executor, RETURN_STRING);
            }

            /// <summary>
            /// Scrolls to the given position.
            /// </summary>
            /// <param name="scrollPosition">The position to scroll to.</param>
            /// <param name="executor">The executor to use.</param>
            /// <returns>The actual position the element had scrolled to.</returns>
            public static Point ScrollTo(Point scrollPosition, IEyesJsExecutor executor)
            {
                return ScrollTo_(scrollPosition, executor, RETURN_STRING);
            }

            /// <summary>
            /// Scrolls current frame to its bottom right.
            /// </summary>
            /// <param name="executor">The executor to use.</param>
            public static void ScrollToBottomRight(IEyesJsExecutor executor)
            {
                executor.ExecuteScript(JSScrollToBottomRight_);
            }

            /// <summary>
            /// CSS-translates the document body to the given position, and returns the previous
            /// transform value.
            /// </summary>
            /// <param name="position">The position to translate to.</param>
            /// <param name="executor">A JavaScript executor.</param>
            /// <returns>The previous transform value</returns>
            public static object TranslateTo(Point position, IEyesJsExecutor executor)
            {
                return TranslateTo_(position, executor, RETURN_STRING);
            }

            /// <summary>
            /// Set the overflow of the document's body, and return the previous overflow value.
            /// </summary>
            /// <param name="overflow">The overflow to set.</param>
            /// <param name="jsExecutor"></param>
            /// <param name="rootElement">Can be either 'body' or 'documentElement'. Default is 'body'.</param>
            /// <returns>The previous overflow value.</returns>
            public static string SetOverflow(string overflow, IEyesJsExecutor jsExecutor, string rootElement = null)
            {
                return SetOverflow_(overflow, jsExecutor, RETURN_STRING, rootElement);
            }

            public static string HideScrollbars(IEyesJsExecutor jsExecutor, string rootElement = null)
            {
                return HideScrollbars_(jsExecutor, RETURN_STRING, rootElement);
            }

            public static string SelectRootElement(IEyesJsExecutor executeScript)
            {
                return SelectRootElement_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Sets a transform on the document's body, and return the previous transform value.
            /// </summary>
            /// <param name="transform">The transform to set.</param>
            /// <param name="executeScript">A reference to a function for executing the script.</param>
            /// <returns>The previous transform value.</returns>
            public static object SetTranform(string transform, IEyesJsExecutor executeScript)
            {
                return SetTranform_(transform, executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Gets current document body's transform string.
            /// </summary>
            /// <returns>The transform string of the document body.</returns>
            public static object GetCurrentTransform(IEyesJsExecutor executeScript)
            {
                return GetCurrentTransform_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Get the size of the entire page based on the scroll width/height.
            /// </summary>
            /// <returns>The size of the entire page.</returns>
            public static Size GetEntirePageSize(IEyesJsExecutor executeScript)
            {
                return GetEntirePageSize_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Get the size of the entire page based on the scroll width/height.
            /// </summary>
            /// <returns>The size of the entire page.</returns>
            public static Size GetEntireElementSize(IEyesJsExecutor executeScript, object element)
            {
                return GetEntireElementSize_(executeScript, RETURN_STRING, element);
            }

            /// <summary>
            /// Get the size of the entire page based on the scroll width/height.
            /// </summary>
            /// <returns>The size of the entire frame content.</returns>
            public static Size GetCurrentFrameContentEntireSize(IEyesJsExecutor executeScript)
            {
                return GetCurrentFrameContentEntireSize_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Gets the useragent string from the browser
            /// </summary>
            /// <returns>useragent string</returns>
            public static object GetUserAgent(Func<string, object> executeScript)
            {
                return GetUserAgent_(executeScript, RETURN_STRING);
            }

            public static Size GetViewportSize(Func<string, object> executeScript)
            {
                return GetViewportSize_(executeScript, RETURN_STRING);
            }

            public static object GetDevicePixelRatio(IEyesJsExecutor executeScript)
            {
                return GetDevicePixelRatio_(executeScript, RETURN_STRING);
            }
        }

        public static class WithoutReturn
        {
            private const string RETURN_STRING = "";
            /// <summary>
            /// Get the scroll position of the current frame.
            /// </summary>
            /// <param name="executor">A JavaScript executor.</param>
            /// <returns>The scroll position of the current frame.</returns>
            public static Point GetCurrentScrollPosition(IEyesJsExecutor executor)
            {
                return GetCurrentScrollPosition_(executor, RETURN_STRING);
            }

            /// <summary>
            /// Scrolls to the given position.
            /// </summary>
            /// <param name="scrollPosition">The position to scroll to.</param>
            /// <param name="executor">A JavaScript executor.</param>
            /// <returns>The actual position the element had scrolled to.</returns>
            public static Point ScrollTo(Point scrollPosition, IEyesJsExecutor executor)
            {
                return ScrollTo_(scrollPosition, executor, RETURN_STRING);
            }

            /// <summary>
            /// Scrolls current frame to its bottom right.
            /// </summary>
            /// <param name="executor">The executor to use.</param>
            public static void ScrollToBottomRight(IEyesJsExecutor executor)
            {
                executor.ExecuteScript(JSScrollToBottomRight_);
            }

            /// <summary>
            /// CSS-translates the document body to the given position, and returns the previous
            /// transform value.
            /// </summary>
            /// <param name="position">The position to translate to.</param>
            /// <param name="executor">A JavaScript executor.</param>
            /// <returns>The previous transform value</returns>
            public static object TranslateTo(Point position, IEyesJsExecutor executor)
            {
                return TranslateTo_(position, executor, RETURN_STRING);
            }

            /// <summary>
            /// Set the overflow of the document's body, and return the previous overflow value.
            /// </summary>
            /// <param name="overflow">The overflow to set.</param>
            /// <param name="jsExecutor"></param>
            /// <returns>The previous overflow value.</returns>
            public static object SetOverflow(string overflow, IEyesJsExecutor jsExecutor)
            {
                return SetOverflow_(overflow, jsExecutor, RETURN_STRING);
            }

            /// <summary>
            /// Hides the scrollbars.
            /// </summary>
            /// <param name="jsExecutor"></param>
            /// <returns>The original overflow value of the page.</returns>
            public static object HideScrollbars(IEyesJsExecutor jsExecutor)
            {
                return HideScrollbars_(jsExecutor, RETURN_STRING);
            }

            /// <summary>
            /// Sets a transform on the document's body, and return the previous transform value.
            /// </summary>
            /// <param name="transform">The transform to set.</param>
            /// <param name="executor">A JavaScript executor.</param>
            /// <returns>The previous transform value.</returns>
            public static object SetTranform(string transform, IEyesJsExecutor executor)
            {
                return SetTranform_(transform, executor, RETURN_STRING);
            }

            /// <summary>
            /// Gets current document body's transform string.
            /// </summary>
            /// <returns>The transform string of the document body.</returns>
            public static object GetCurrentTransform(IEyesJsExecutor executor)
            {
                return GetCurrentTransform_(executor, RETURN_STRING);
            }

            /// <summary>
            /// Get the size of the entire page based on the scroll width/height.
            /// </summary>
            /// <returns>The size of the entire page.</returns>
            public static Size GetEntirePageSize(IEyesJsExecutor executeScript)
            {
                return GetEntirePageSize_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Get the size of the entire page based on the scroll width/height.
            /// </summary>
            /// <returns>The size of the entire frame content.</returns>
            public static Size GetCurrentFrameContentEntireSize(IEyesJsExecutor executeScript)
            {
                return GetCurrentFrameContentEntireSize_(executeScript, RETURN_STRING);
            }

            /// <summary>
            /// Gets the useragent string from the browser
            /// </summary>
            /// <returns>useragent string</returns>
            public static object GetUserAgent(Func<string, object> executeScript)
            {
                return GetUserAgent_(executeScript, RETURN_STRING);
            }

            public static Size GetViewportSize(Func<string, object> executeScript)
            {
                return GetViewportSize_(executeScript, RETURN_STRING);
            }

            public static object GetDevicePixelRatio(IEyesJsExecutor executeScript)
            {
                return GetDevicePixelRatio_(executeScript, RETURN_STRING);
            }
        }

        #endregion Types

        #region Methods

        /// <summary>
        /// Get the scroll position of the current frame.
        /// </summary>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The scroll position of the current frame.</returns>
        private static Point GetCurrentScrollPosition_(IEyesJsExecutor executor, string returnStr)
        {
            var position = executor.ExecuteScript(JSGetScrollPosition_.Replace(RETURN_PLACEHOLDER_, returnStr));
            return ParseLocation(position, "Could not get scroll position!");
        }

        private static Point ParseLocation(object position, string error)
        {
            var xy = position.ToString().Split(',');
            if (xy.Length != 2 ||
                !float.TryParse(xy[0], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float x) ||
                !float.TryParse(xy[1], NumberStyles.Float, NumberFormatInfo.InvariantInfo, out float y))
            {
                throw new EyesException(error);
            }

            return new Point((int)Math.Ceiling(x), (int)Math.Ceiling(y));
        }

        /// <summary>
        /// Scrolls to the given position.
        /// </summary>
        /// <param name="scrollPosition">The position to scroll to.</param>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The actual position the element had scrolled to.</returns>
        public static Point ScrollTo_(Point scrollPosition, IEyesJsExecutor executor, string returnStr)
        {
            var position = executor.ExecuteScript($"window.scrollTo({scrollPosition.X},{scrollPosition.Y});" + JSGetScrollPosition_.Replace(RETURN_PLACEHOLDER_, returnStr));
            return ParseLocation(position, "Could not get scroll position!");
        }

        /// <summary>
        /// CSS-translates the document body to the given position, and returns the previous
        /// transform value.
        /// </summary>
        /// <param name="position">The position to translate to.</param>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The previous transform value</returns>
        public static object TranslateTo_(Point position, IEyesJsExecutor executor,
            string returnStr)
        {
            Point negatedPos = new Point(-position.X, -position.Y);

            string transformStr = /*position.IsEmpty ? "none" : */ $"translate({negatedPos.X}px, {negatedPos.Y}px)";
            return SetTranform_(transformStr, executor, returnStr);
        }

        /// <summary>
        /// Set the overflow of the document's body, and return the previous overflow value.
        /// </summary>
        /// <param name="overflow">The overflow to set.</param>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <param name="rootElement">Can be either 'body' or 'documentElement'. Default is 'body'.</param>
        /// <returns>The previous overflow value.</returns>
        public static string SetOverflow_(string overflow, IEyesJsExecutor executor,
            string returnStr, string rootElement = null)
        {
            string returnAppliedStr = JSSetOverflow_.Replace(RETURN_PLACEHOLDER_, returnStr);
            string overflowStr = returnAppliedStr.Replace("{0}", overflow).Replace("{1}", rootElement ?? "documentElement");
            var result = executor.ExecuteScript(overflowStr);
            Thread.Sleep(SetOverflowWaitMS_);
            return result as string;
        }

        /// <summary>
        /// Hides the scrollbars of the page, and returns the original overflow value.
        /// </summary>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <param name="rootElement">Can be either 'body' or 'documentElement'. Default is 'body'.</param>
        /// <returns>The original overflow value of the page.</returns>
        public static string HideScrollbars_(IEyesJsExecutor executor, string returnStr, string rootElement = null)
        {
            return SetOverflow_("hidden", executor, returnStr, rootElement);
        }

        public static string SelectRootElement_(IEyesJsExecutor executeScript, string returnStr)
        {
            //string returnAppliedStr = JSSelectRootElement_.Replace(RETURN_PLACEHOLDER_, returnStr);
            //var result = executeScript.ExecuteScript(returnAppliedStr);
            //return result as string;
            return "documentElement";
        }

        /// <summary>
        /// Sets a transform on the document's body, and return the previous transform value.
        /// </summary>
        /// <param name="transform">The transform to set.</param>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The previous transform value.</returns>
        public static object SetTranform_(string transform, IEyesJsExecutor executor,
            string returnStr)
        {
            string returnAppliedStr = JSSetTransform_.Replace(RETURN_PLACEHOLDER_, returnStr);
            string transformStr = returnAppliedStr.Replace("{0}", transform);
            return executor.ExecuteScript(transformStr);
        }

        /// <summary>
        /// Gets current document body's transform string.
        /// </summary>
        /// <param name="executor">A JavaScript executor.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The transform string of the document body.</returns>
        public static object GetCurrentTransform_(IEyesJsExecutor executor,
            string returnStr)
        {
            string returnAppliedStr =
                JSGetCurrentTransform_.Replace(RETURN_PLACEHOLDER_, returnStr);
            return executor.ExecuteScript(returnAppliedStr);
        }

        /// <summary>
        /// Get the size of the entire page based on the scroll width/height.
        /// </summary>
        /// <param name="executeScript">A reference to a function for executing the script.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The size of the entire page.</returns>
        public static Size GetEntirePageSize_(IEyesJsExecutor executeScript, string returnStr)
        {
            var size = executeScript.ExecuteScript(JSGetEntirePageSize_.Replace(RETURN_PLACEHOLDER_, returnStr));
            return new Size(ParseLocation(size, "Could not get entire page size!"));
        }


        /// <summary>
        /// Get the size of the entire element based on the scroll width/height.
        /// </summary>
        /// <param name="executeScript">A reference to a function for executing the script.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <param name="element">The element for which to take the size.</param>
        /// <returns>The size of the entire page.</returns>
        public static Size GetEntireElementSize_(IEyesJsExecutor executeScript, string returnStr, object element)
        {
            var size = executeScript.ExecuteScript(JSGetEntireElementSize_.Replace(RETURN_PLACEHOLDER_, returnStr), element);
            return new Size(ParseLocation(size, "Could not get entire page size!"));
        }

        /// <summary>
        /// Get the size of the entire page based on the scroll width/height.
        /// </summary>
        /// <param name="executeScript">A reference to a function for executing the script.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>The size of the entire page.</returns>
        public static Size GetCurrentFrameContentEntireSize_(IEyesJsExecutor executeScript, string returnStr)
        {
            var size = executeScript.ExecuteScript(JSReturnContentEntireSize_.Replace(RETURN_PLACEHOLDER_, returnStr));
            return new Size(ParseLocation(size, "Could not get entire page size!"));
        }

        /// <summary>
        /// Gets the useragent string from the browser
        /// </summary>
        /// <param name="executeScript">A reference to a function for executing the script.</param>
        /// <param name="returnStr">The return string which the script should use.</param>
        /// <returns>useragent string</returns>
        private static object GetUserAgent_(Func<string, object> executeScript, string returnStr)
        {
            return executeScript(returnStr + " navigator.userAgent;");
        }

        private static Size GetViewportSize_(Func<string, object> executeScript, string returnStr)
        {
            var jsonWidth = executeScript(JSGetViewportWidth_.Replace(RETURN_PLACEHOLDER_, returnStr));
            var jsonHeight = executeScript(JSGetViewportHeight_.Replace(RETURN_PLACEHOLDER_, returnStr));
            if (jsonWidth != null && jsonHeight != null)
            {
                var width = jsonWidth.ToString().ToInt32();
                var height = jsonHeight.ToString().ToInt32();
                return new Size(width, height);
            }

            return new Size(0, 0);
        }

        /// <summary>
        /// Gets the device pixel ratio of the device running the hosting app.
        /// </summary>
        /// <returns>The Device pixel ratio.</returns>
        private static object GetDevicePixelRatio_(IEyesJsExecutor executeScript,
            string returnStr)
        {
            return executeScript.ExecuteScript(returnStr + " window.devicePixelRatio;");
        }

        #endregion Methods
    }
}
