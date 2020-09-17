namespace Applitools.Selenium.Tests.Utils
{
    using System;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Applitools.Tests.Utils;
    using Applitools.Utils;
    using Applitools.VisualGrid;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Edge;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.IE;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Safari;
    using OpenQA.Selenium.Support.UI;

    /// <summary>
    /// Selenium WebDriver related utilities.
    /// </summary>
    public static class SeleniumUtils
    {
        private const int DefaultTimeoutInSeconds_ = 30;
        private const string TestApiIndicatorsContainerId_ = "Test";

        public static readonly string DRIVER_PATH = Environment.GetEnvironmentVariable("DRIVER_PATH");

        public static ChromeDriver CreateChromeDriver(ChromeOptions options = null)
        {
            if (options == null)
            {
                options = new ChromeOptions();
            }
            if (TestUtils.RUN_HEADLESS)
            {
                options.AddArgument("--headless");
            }

            ChromeDriver webDriver = RetryCreateWebDriver(() =>
            {
                ChromeDriver driver = DRIVER_PATH != null ? new ChromeDriver(DRIVER_PATH, options) : new ChromeDriver(options);
                return driver;

            });
            return webDriver;
        }

        public static IWebDriver CreateWebDriver(DriverOptions options)
        {
            IWebDriver webDriver = RetryCreateWebDriver(() =>
            {
                IWebDriver driver = null;
                switch (options.BrowserName)
                {
                    case "firefox": driver = DRIVER_PATH != null ? new FirefoxDriver(DRIVER_PATH, (FirefoxOptions)options) : new FirefoxDriver((FirefoxOptions)options); break;
                    case "safari": driver = DRIVER_PATH != null ? new SafariDriver(DRIVER_PATH, (SafariOptions)options) : new SafariDriver((SafariOptions)options); break;
                    case "internet explorer": driver = DRIVER_PATH != null ? new InternetExplorerDriver(DRIVER_PATH, (InternetExplorerOptions)options) : new InternetExplorerDriver((InternetExplorerOptions)options); break;
                    case "edge": driver = DRIVER_PATH != null ? new EdgeDriver(DRIVER_PATH, (EdgeOptions)options) : new EdgeDriver((EdgeOptions)options); break;
                    default: driver = DRIVER_PATH != null ? new ChromeDriver(DRIVER_PATH, (ChromeOptions)options) : new ChromeDriver((ChromeOptions)options); break;
                }
                return driver;
            });
            return webDriver;
        }

        public static T RetryCreateWebDriver<T>(Func<T> func, int times = 3) where T : class, IWebDriver
        {
            int retriesLeft = times;
            int wait = 500;
            while (retriesLeft-- > 0)
            {
                try
                {
                    T result = func.Invoke();
                    if (result != null) return result;
                }
                catch (Exception)
                {
                    if (retriesLeft == 0) throw;
                }
                Thread.Sleep(wait);
                wait *= 2;
                wait = Math.Min(10000, wait);
            }
            return null;
        }

        /// <summary>
        /// Moves the mouse pointer over offset from the element.
        /// </summary>
        public static void MoveToElement(
            IWebDriver driver, IWebElement element, int offsetX, int offsetY)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(element, nameof(element));

            Actions builder = new Actions(driver);
            builder.MoveToElement(element, offsetX, offsetY).Build().Perform();
        }

        /// <summary>
        /// Moves the mouse over to the middle of the element.
        /// </summary>
        public static void MoveToElement(IWebDriver driver, IWebElement element)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(element, nameof(element));

            /// we want to hover on the middle of the element
            int offsetX = element.Size.Width / 2;
            int offsetY = element.Size.Height / 2;

            MoveToElement(driver, element, offsetX, offsetY);
        }

        /// <summary>
        /// Hovers over the middle of the input element for the input amount of
        /// milliseconds.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="timeToHover"></param>
        public static void HoverOverElement(IWebDriver driver, IWebElement element, int timeToHover)
        {
            MoveToElement(driver, element);
            Thread.Sleep(timeToHover);
        }

        /// <summary>
        /// Hovers over the middle of the input element for 2 seconds.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        public static void HoverOverElement(IWebDriver driver, IWebElement element)
        {
            HoverOverElement(driver, element, 2000);
        }

        /// <summary>
        /// Moves the mouse cursor to the top left corner of the browser window
        /// </summary>
        /// <param name="driver"></param>
        public static void MoveToTopLeft(IWebDriver driver)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));

            IWebElement body = driver.FindElement(By.TagName("body"));
            MoveToElement(driver, body, 0, 0);
        }

        /// <summary>
        /// Drags the input element to specified offset.
        /// </summary>
        public static void DragElement(
            IWebDriver driver, IWebElement element, int offsetX, int offsetY)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(element, nameof(element));

            Actions builder = new Actions(driver);
            builder.DragAndDropToOffset(element, offsetX, offsetY).Build().Perform();
        }

        /// <summary>
        /// Waits for an element be found in the DOM by the locator.
        /// </summary>
        public static IWebElement WaitForElement(IWebDriver driver, By locator)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(locator, nameof(locator));

            return (new WebDriverWait(driver, new TimeSpan(0, 0, 0, DefaultTimeoutInSeconds_)))
                    .Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(locator));
        }

        /// <summary>
        /// Clicks on coordinates based on offset from a container element.
        /// </summary>
        public static void Click(
            IWebDriver driver, IWebElement container, Point offsetFromContainer)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(container, nameof(container));

            Actions builder = new Actions(driver);
            builder.MoveToElement(
                container,
                offsetFromContainer.X,
                offsetFromContainer.Y).Click().Build().Perform();
        }

        /// <summary>
        /// Double clicks the input element.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        public static void DoubleClick(IWebDriver driver, IWebElement element)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(element, nameof(element));

            Actions builder = new Actions(driver);
            builder.MoveToElement(element).DoubleClick().Build().Perform();
        }

        /// <summary>
        /// Clicks the location indicated by the specified offset from the top-left
        /// corner of the specified element, and moves the cursor to the top-left
        /// corner of the screen to avoid hover artifacts.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="offset"></param>
        public static void ClickAndMove(IWebDriver driver, IWebElement element, Point offset)
        {
            Click(driver, element, offset);
            SeleniumUtils.MoveToTopLeft(driver);
        }

        /// <summary>
        /// Clicks the input element and moves the cursor to the top-left corner of
        /// the screen to avoid hover artifacts.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        public static void ClickAndMove(IWebDriver driver, IWebElement element)
        {
            element.Click();
            SeleniumUtils.MoveToTopLeft(driver);
        }

        /// <summary>
        /// Clicks the location indicated by the specified offset from the top-left
        /// corner of the specified element, and moves the cursor to the top-left
        /// corner of the screen to avoid hover artifacts, and waits for the
        /// specified element to load.
        /// </summary>
        public static IWebElement ClickMoveAndWait(
            IWebDriver driver, IWebElement element, Point offset, By waitFor)
        {
            ClickAndMove(driver, element, offset);
            return WaitForElement(driver, waitFor);
        }

        /// <summary>
        /// Clicks the location indicated by the specified offset from the top-left
        /// corner of the specified element, and moves the cursor to the top-left
        /// corner of the screen to avoid hover artifacts, and waits for the
        /// specified element to load.
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="element"></param>
        /// <param name="offset"></param>
        /// <param name="waitFor"></param>
        /// <returns>The element waited for.</returns>
        public static IWebElement ClickAndWait(
            IWebDriver driver, IWebElement element, Point offset, By waitFor)
        {
            Click(driver, element, offset);
            MoveToElement(driver, element, 0, -20);
            return WaitForElement(driver, waitFor);
        }

        /// <summary>
        /// Scrolls up/down by using the <c>mousewheel</c> java script event.
        /// </summary>
        /// <param name="driver">The WebDriver used for automating the browser.</param>
        /// <param name="element">
        /// The element which will dispatch the <c>mousewheel</c> event.</param>
        /// <param name="deltaY">
        /// Positive value for scrolling up, negative for scrolling down.</param>
        public static void ScrollElement(IWebDriver driver, IWebElement element, int deltaY)
        {
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(element, nameof(element));

            string scrollScript = "scrollEvent = document.createEvent(\"MouseEvents\");";
            scrollScript += "scrollableElement = arguments[0];";
            scrollScript += "scrollEvent.initEvent(\"mousewheel\",true,true);";
            scrollScript += "scrollEvent.wheelDeltaY = arguments[1];";
            scrollScript += "scrollableElement.dispatchEvent(scrollEvent);";

            ((IJavaScriptExecutor)driver).ExecuteScript(scrollScript, element, deltaY);
        }

        /// <summary>
        /// Waits for the event of the input name and clears it.
        /// </summary>
        public static void WaitForEvent(IWebDriver driver, string eventName)
        {
            WaitForElement(driver, GetEventLocator_(eventName));
            ResetEvent_(driver, eventName);
        }

        private static void ResetEvent_(IWebDriver driver, string eventName)
        {
            var eventCssSelector = GetEventCssSelector_(eventName);
            ((IJavaScriptExecutor)driver).ExecuteScript(
                    "events = arguments[0]; $(events).find('" +
                    eventCssSelector + "').remove()",
                    GetEventsContainerElement_(driver));
        }

        private static IWebElement GetEventsContainerElement_(IWebDriver driver)
        {
            return driver.FindElement(By.Id(TestApiIndicatorsContainerId_));
        }

        private static By GetEventLocator_(string eventName)
        {
            return By.CssSelector(GetEventCssSelector_(eventName));
        }

        private static string GetEventCssSelector_(string eventName)
        {
            return "[data-test-event=" + eventName + "]";
        }

        internal static void InitEyes(bool useVisualGrid, out IWebDriver driver, out EyesRunner runner, out Eyes eyes, [CallerMemberName] string testName = null)
        {
            driver = CreateChromeDriver();
            if (useVisualGrid)
            {
                testName += "_VG";
                VisualGridRunner visualGridRunner = new VisualGridRunner(10);
                //visualGridRunner.DebugResourceWriter = new FileDebugResourceWriter(logPath);
                runner = visualGridRunner;
            }
            else
            {
                runner = new ClassicRunner();
            }

            string logPath = TestUtils.InitLogPath(testName);
            eyes = new Eyes(runner);
            eyes.SetLogHandler(TestUtils.InitLogHandler(testName, logPath));
            eyes.Batch = TestDataProvider.BatchInfo;
        }

    }
}
