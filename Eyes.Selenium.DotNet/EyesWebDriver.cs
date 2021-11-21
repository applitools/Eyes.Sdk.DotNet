using Applitools.OpenQA.Selenium;
using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using Size = System.Drawing.Size;

namespace Applitools.Selenium
{
    public sealed class EyesWebDriver : IWebDriver, ITakesScreenshot, IJavaScriptExecutor, IEyesJsExecutor
    // IHasCapabilities, ISearchContext, IActionExecutor
    {
        #region Fields

        private Size defaultContentViewportSize_;
        private ITargetLocator targetLocator_;
        private readonly FrameChain frameChain_;
        private Dictionary<string, IWebElement> elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
        private IEyesWebDriverAdapter eyesWebDriverAdapter_;
        #endregion

        #region Constructors

        internal EyesWebDriver(Logger logger, SeleniumEyes eyes, IUserActionsEyes userActionEyes, IWebDriver driver)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            //ArgumentGuard.NotNull(eyes, nameof(eyes));
            ArgumentGuard.NotNull(driver, nameof(driver));

            Logger_ = logger;
            Eyes = eyes;
            UserActionsEyes = userActionEyes;
            WebDriver = driver;
            frameChain_ = new FrameChain(logger);
            var driverType = driver.GetType();
            bool isAppiumDriver = false;
            var dt = driverType;
            while (dt != null && !isAppiumDriver)
            {
                isAppiumDriver = dt.Name.StartsWith("AppiumDriver`");
                dt = dt.BaseType;
            }

            if (!isAppiumDriver)
            {
                eyesWebDriverAdapter_.TryAddCommand("getSession", "GET", "/session/{sessionId}/");
            }

            Logger_.Verbose("Driver is {0}", driver.GetType());
        }

        public FrameChain GetFrameChain()
        {
            return frameChain_;
        }

        #endregion

        #region Properties

        internal SeleniumEyes Eyes { get; private set; }
        internal IUserActionsEyes UserActionsEyes { get; private set; }

        public IWebDriver WebDriver { get; internal set; }

        public string Title
        {
            get { return WebDriver.Title; }
        }

        public string PageSource
        {
            get { return WebDriver.PageSource; }
        }

        public ReadOnlyCollection<string> WindowHandles
        {
            get { return WebDriver.WindowHandles; }
        }

        public string CurrentWindowHandle
        {
            get { return WebDriver.CurrentWindowHandle; }
        }

        public ICapabilities Capabilities
        {
            get { return WebDriver.Capabilities; }
        }

        public string Url
        {
            get { return WebDriver.Url; }
            set
            {
                WebDriver.Url = value;
                elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }
        }

        private Logger Logger_ { get; set; }

        public bool IsActionExecutor => WebDriver.IsActionExecutor;

        #endregion

        #region Methods

        public string GetUserAgent()
        {
            string userAgent = null;

            try
            {
                userAgent = (string)JSBrowserCommands.WithReturn.GetUserAgent((s) => ExecuteScript(s));
            }
            catch (Exception)
            {
                Logger_.Log(TraceLevel.Error, Eyes.TestId, Stage.Open, new { message = "Failed to obtain user-agent string" });
            }

            return userAgent;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            IEnumerable<IWebElement> foundWebElementsList = WebDriver.FindElements(by);

            // This list will contain the found elements wrapped with our class.
            List<IWebElement> eyesWebElementsList = new List<IWebElement>();

            // TODO - Daniel, Support additional implementation of web element
            foreach (IWebElement element in foundWebElementsList)
            {
                if (element is IWebElement && !(element is EyesWebElement))
                {
                    EyesWebElement erwe = new EyesWebElement(Logger_, this, element);
                    eyesWebElementsList.Add(erwe);
                    string id = EyesSeleniumUtils.GetElementIdForDictionary(element, WebDriver);
                    elementsFoundSinceLastNavigation_[id] = erwe;
                }
                else
                {
                    eyesWebElementsList.Add(element);
                    string id = EyesSeleniumUtils.GetElementIdForDictionary(element, WebDriver);
                    elementsFoundSinceLastNavigation_[id] = element;
                }
            }
            return eyesWebElementsList.AsReadOnly();
        }

        public IWebElement FindElement(By by)
        {
            // TODO - Daniel, support additional implementations of WebElement.
            IWebElement webElement = WebDriver.FindElement(by);
            string id;
            if (webElement is IWebElement WebElement && !(webElement is EyesWebElement))
            {
                webElement = new EyesWebElement(Logger_, this, WebElement);
            }
            else if (webElement == null)
            {
                throw new EyesException($"Element not found: {by}");
            }
            id = EyesSeleniumUtils.GetElementIdForDictionary(webElement, WebDriver);
            elementsFoundSinceLastNavigation_[id] = webElement;
            return webElement;
        }

        public void Close()
        {
            WebDriver.Close();
        }

        public void Quit()
        {
            WebDriver.Quit();
        }

        public ITargetLocator SwitchTo()
        {
            if (targetLocator_ == null)
            {
                targetLocator_ = new EyesWebDriverTargetLocator(this, Logger_, WebDriver.SwitchTo());
            }
            return targetLocator_;
        }

        private class EyesWebDriverNavigation : INavigation
        {
            private INavigation navigation_;
            private FrameChain frameChain_;
            private EyesWebDriver driver_;

            public EyesWebDriverNavigation(INavigation navigation, FrameChain frameChain, EyesWebDriver driver)
            {
                navigation_ = navigation;
                frameChain_ = frameChain;
                driver_ = driver;
            }

            public void Back()
            {
                navigation_.Back();
                frameChain_.Clear();
                driver_.elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }

            public void Forward()
            {
                navigation_.Forward();
                frameChain_.Clear();
                driver_.elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }

            public void GoToUrl(Uri url)
            {
                navigation_.GoToUrl(url);
                frameChain_.Clear();
                driver_.elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }

            public void GoToUrl(string url)
            {
                navigation_.GoToUrl(url);
                frameChain_.Clear();
                driver_.elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }

            public void Refresh()
            {
                navigation_.Refresh();
                frameChain_.Clear();
                driver_.elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }
        }

        public INavigation Navigate()
        {
            return new EyesWebDriverNavigation(WebDriver.Navigate(), frameChain_, this);
        }

        public IOptions Manage()
        {
            return WebDriver.Manage();
        }

        public IWebElement FindElementByClassName(string className)
        {
            return FindElement(By.ClassName(className));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByClassName(string className)
        {
            return FindElements(By.ClassName(className));
        }

        public IWebElement FindElementByCssSelector(string cssSelector)
        {
            return FindElement(By.CssSelector(cssSelector));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByCssSelector(string cssSelector)
        {
            return FindElements(By.CssSelector(cssSelector));
        }

        public IWebElement FindElementById(string id)
        {
            return FindElement(By.Id(id));
        }

        public ReadOnlyCollection<IWebElement> FindElementsById(string id)
        {
            return FindElements(By.Id(id));
        }

        public IWebElement FindElementByLinkText(string linkText)
        {
            return FindElement(By.LinkText(linkText));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByLinkText(string linkText)
        {
            return FindElements(By.LinkText(linkText));
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "Serialization required")]
        public IWebElement FindElementByPartialLinkText(string partialLinkText)
        {
            return FindElement(By.PartialLinkText(partialLinkText));
        }

        public IWebElement FindElementByName(string name)
        {
            return FindElement(By.Name(name));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByName(string name)
        {
            return FindElements(By.Name(name));
        }

        public IWebElement FindElementByTagName(string tagName)
        {
            return FindElement(By.TagName(tagName));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByTagName(string tagName)
        {
            return FindElements(By.TagName(tagName));
        }

        public IWebElement FindElementByXPath(string xpath)
        {
            return FindElement(By.XPath(xpath));
        }

        public ReadOnlyCollection<IWebElement> FindElementsByXPath(string xpath)
        {
            return FindElements(By.XPath(xpath));
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)WebDriver).ExecuteScript(script, args);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return ((IJavaScriptExecutor)WebDriver).ExecuteAsyncScript(script, args);
        }

        internal IDictionary<string, object> SessionDetails => eyesWebDriverAdapter_.GetSessionDetails();

        public string SessionId => throw new NotImplementedException();

        internal object GetSessionDetail(string detail)
        {
            IDictionary<string, object> sessionDetails = SessionDetails;
            if (sessionDetails == null) return null;
            return sessionDetails.ContainsKey(detail) ? sessionDetails[detail] : null;
        }

        /// <summary>
        /// Returns the viewport size of the default content (outer most frame).
        /// </summary>
        /// <param name="forceQuery">If true, we will perform the query even if we have a cached viewport size.</param>
        /// <returns>The viewport size of the default content (outer most frame).</returns>
        public Size GetDefaultContentViewportSize(bool forceQuery = false)
        {
            Logger_.Verbose("GetDefaultContentViewportSize()");

            if (!defaultContentViewportSize_.IsEmpty && !forceQuery)
            {
                Logger_.Verbose("Using cached viewport size: {0}", defaultContentViewportSize_);
                return defaultContentViewportSize_;
            }

            FrameChain currentFrames = frameChain_.Clone();
            if (currentFrames.Count > 0)
            {
                SwitchTo().DefaultContent();
            }

            Logger_.Verbose("Extracting viewport size...");
            defaultContentViewportSize_ = EyesSeleniumUtils.GetViewportSizeOrDisplaySize(Logger_, Eyes.TestId, this);
            Logger_.Verbose("Done! Viewport size: {0}", defaultContentViewportSize_);

            if (currentFrames.Count > 0)
            {
                ((EyesWebDriverTargetLocator)SwitchTo()).Frames(currentFrames);
            }
            return defaultContentViewportSize_;
        }

        public Screenshot GetScreenshot()
        {
            return ((ITakesScreenshot)WebDriver).GetScreenshot();
        }

        public void Dispose()
        {
            WebDriver.Dispose();
        }

        public void PerformActions(IList<IActionSequence> actionSequenceList)
        {
            ((IActionExecutor)WebDriver).PerformActions(actionSequenceList);
            foreach (IActionSequence actionSequence in actionSequenceList)
            {
                Dictionary<string, object> actions = actionSequence.ToDictionary();
                AddTrigger_(actions);
            }
        }

        private void AddTrigger_(Dictionary<string, object> action)
        {
            IDictionary<string, object> parameters = (IDictionary<string, object>)action["parameters"];
            if ("pointer".Equals(action["type"]) && "mouse".Equals(parameters["pointerType"]))
            {
                IList<object> actions = (IList<object>)action["actions"];
                foreach (Dictionary<string, object> actionDictionary in actions)
                {
                    if ("pointerMove".Equals(actionDictionary["type"]))
                    {
                        int x = (int)actionDictionary["x"];
                        int y = (int)actionDictionary["y"];
                        IDictionary<string, object> originDict = (IDictionary<string, object>)actionDictionary["origin"];
                        string elementId = originDict.Values.FirstOrDefault().ToString() + "_" + WebDriver.SessionId;
                        if (elementsFoundSinceLastNavigation_.TryGetValue(elementId, out IWebElement element))
                        {
                            UserActionsEyes.AddMouseTrigger(MouseAction.Move, element, new Point(x, y));
                        }
                    }
                    else
                    {

                    }
                }
            }
        }

        public void ResetInputState()
        {
            WebDriver.ResetInputState();
        }

        IOptions IWebDriver.Manage()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}