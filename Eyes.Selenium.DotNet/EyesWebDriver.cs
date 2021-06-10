using Applitools.Utils;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Size = System.Drawing.Size;

namespace Applitools.Selenium
{
    public sealed class EyesWebDriver :
        IHasCapabilities,
#pragma warning disable CS0618 // Type or member is obsolete
        IHasInputDevices,
#pragma warning restore CS0618 // Type or member is obsolete
        IFindsByClassName, IFindsByCssSelector,
        IFindsById, IFindsByLinkText, IFindsByName, IFindsByTagName, IFindsByXPath,
        IJavaScriptExecutor, ISearchContext, ITakesScreenshot, IWebDriver, IActionExecutor,
        IEyesJsExecutor
    {
        #region Fields

        private Size defaultContentViewportSize_;
        private ITargetLocator targetLocator_;
        private readonly FrameChain frameChain_;
        private Dictionary<string, IWebElement> elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
        private MethodInfo executeCommandMI_;
        #endregion

        #region Constructors

        internal EyesWebDriver(Logger logger, SeleniumEyes eyes, IUserActionsEyes userActionEyes, RemoteWebDriver driver)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            //ArgumentGuard.NotNull(eyes, nameof(eyes));
            ArgumentGuard.NotNull(driver, nameof(driver));

            Logger_ = logger;
            Eyes = eyes;
            UserActionsEyes = userActionEyes;
            RemoteWebDriver = driver;
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
                executeCommandMI_ = driverType.GetMethod("Execute", BindingFlags.Instance | BindingFlags.NonPublic);
                var commandExecutorProperty = driverType.GetProperty("CommandExecutor", BindingFlags.Instance | BindingFlags.NonPublic);
                var commandExecutor = (ICommandExecutor)commandExecutorProperty.GetValue(driver);
                commandExecutor.CommandInfoRepository.TryAddCommand("getSession", new CommandInfo("GET", "/session/{sessionId}/"));
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

        public RemoteWebDriver RemoteWebDriver { get; internal set; }

        public string Title
        {
            get { return RemoteWebDriver.Title; }
        }

        public string PageSource
        {
            get { return RemoteWebDriver.PageSource; }
        }

        public ReadOnlyCollection<string> WindowHandles
        {
            get { return RemoteWebDriver.WindowHandles; }
        }

        public string CurrentWindowHandle
        {
            get { return RemoteWebDriver.CurrentWindowHandle; }
        }

        [Obsolete]
        public IMouse Mouse
        {
            get { return new EyesMouse(Logger_, this, RemoteWebDriver.Mouse); }
        }

        [Obsolete]
        public IKeyboard Keyboard
        {
            get { return new EyesKeyboard(Logger_, this, RemoteWebDriver.Keyboard); }
        }

        public ICapabilities Capabilities
        {
            get { return RemoteWebDriver.Capabilities; }
        }

        public string Url
        {
            get { return RemoteWebDriver.Url; }
            set
            {
                RemoteWebDriver.Url = value;
                elementsFoundSinceLastNavigation_ = new Dictionary<string, IWebElement>();
            }
        }

        private Logger Logger_ { get; set; }

        public bool IsActionExecutor => RemoteWebDriver.IsActionExecutor;

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
            IEnumerable<IWebElement> foundWebElementsList = RemoteWebDriver.FindElements(by);

            // This list will contain the found elements wrapped with our class.
            List<IWebElement> eyesWebElementsList = new List<IWebElement>();

            // TODO - Daniel, Support additional implementation of web element
            foreach (IWebElement element in foundWebElementsList)
            {
                if (element is RemoteWebElement && !(element is EyesRemoteWebElement))
                {
                    EyesRemoteWebElement erwe = new EyesRemoteWebElement(Logger_, this, element);
                    eyesWebElementsList.Add(erwe);
                    string id = EyesSeleniumUtils.GetElementIdForDictionary(element, RemoteWebDriver);
                    elementsFoundSinceLastNavigation_[id] = erwe;
                }
                else
                {
                    eyesWebElementsList.Add(element);
                    string id = EyesSeleniumUtils.GetElementIdForDictionary(element, RemoteWebDriver);
                    elementsFoundSinceLastNavigation_[id] = element;
                }
            }
            return eyesWebElementsList.AsReadOnly();
        }

        public IWebElement FindElement(By by)
        {
            // TODO - Daniel, support additional implementations of WebElement.
            IWebElement webElement = RemoteWebDriver.FindElement(by);
            string id;
            if (webElement is RemoteWebElement remoteWebElement && !(webElement is EyesRemoteWebElement))
            {
                webElement = new EyesRemoteWebElement(Logger_, this, remoteWebElement);
            }
            else if (webElement == null)
            {
                throw new EyesException($"Element not found: {by}");
            }
            id = EyesSeleniumUtils.GetElementIdForDictionary(webElement, RemoteWebDriver);
            elementsFoundSinceLastNavigation_[id] = webElement;
            return webElement;
        }

        public void Close()
        {
            RemoteWebDriver.Close();
        }

        public void Quit()
        {
            RemoteWebDriver.Quit();
        }

        public ITargetLocator SwitchTo()
        {
            if (targetLocator_ == null)
            {
                targetLocator_ = new EyesWebDriverTargetLocator(this, Logger_, RemoteWebDriver.SwitchTo());
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
            return new EyesWebDriverNavigation(RemoteWebDriver.Navigate(), frameChain_, this);
        }

        public IOptions Manage()
        {
            return RemoteWebDriver.Manage();
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
            return RemoteWebDriver.ExecuteScript(script, args);
        }

        public object ExecuteAsyncScript(string script, params object[] args)
        {
            return RemoteWebDriver.ExecuteAsyncScript(script, args);
        }

        internal IDictionary<string, object> SessionDetails
        {
            get
            {
                try
                {
                    var response = (Response)executeCommandMI_.Invoke(RemoteWebDriver, new object[] { "getSession", null });
                    if (response == null || response.Status != WebDriverResult.Success) return null;
                    if (!(response.Value is IDictionary<string, object> dict)) return null;
                    return dict.Where(entry =>
                    {
                        string key = entry.Key;
                        object value = entry.Value;
                        return !string.IsNullOrEmpty(key) && value != null && !string.IsNullOrEmpty(Convert.ToString(value));
                    })
                    .ToDictionary(entry => entry.Key, entry => entry.Value);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

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
            return ((ITakesScreenshot)RemoteWebDriver).GetScreenshot();
        }

        public void Dispose()
        {
            RemoteWebDriver.Dispose();
        }

        public void PerformActions(IList<ActionSequence> actionSequenceList)
        {
            RemoteWebDriver.PerformActions(actionSequenceList);
            foreach (ActionSequence actionSequence in actionSequenceList)
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
                        string elementId = originDict.Values.FirstOrDefault().ToString() + "_" + RemoteWebDriver.SessionId;
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
            RemoteWebDriver.ResetInputState();
        }

        #endregion
    }
}