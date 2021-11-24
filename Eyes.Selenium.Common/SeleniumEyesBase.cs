using Applitools.Fluent;
using Applitools.Utils;
using Applitools.VisualGrid;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;

namespace Applitools.Selenium.Common
{
    internal abstract class SeleniumEyesBase<IWebElement> : RunningTest, IEyes, IUserActionsEyes<IWebElement>, ISeleniumEyesCommon
    {
        protected const string SET_DATA_APPLITOOLS_SCROLL_ATTR = "var e = arguments[0]; if (e != null) e.setAttribute('data-applitools-scroll','true');";
        protected const string HIDE_CARET = "var activeElement = document.activeElement; activeElement && activeElement.blur(); return activeElement;";

        protected ISeleniumConfigurationProvider configProvider_;
        protected static readonly double UnknownDevicePixelRatio_ = 0;
        protected static readonly double DefaultDevicePixelRatio_ = 1;

        #region Constructors

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes Server at the 
        /// specified url.
        /// </summary>
        /// <param name="serverUrl">The Eyes server URL.</param>
        public SeleniumEyesBase(ISeleniumConfigurationProvider configurationProvider, Uri serverUrl, ClassicRunner runner)
            : base(runner)
        {
            ArgumentGuard.NotNull(serverUrl, nameof(serverUrl));
            configProvider_ = configurationProvider;
            ServerUrl = serverUrl.ToString();
            Init_();
        }

        /// <summary>
        /// Creates a new Eyes instance that interacts with the Eyes cloud service.
        /// </summary>
        public SeleniumEyesBase(ISeleniumConfigurationProvider configurationProvider, ClassicRunner runner)
            : base(runner)
        {
            configProvider_ = configurationProvider;
            runner_ = runner;
            Init_();
        }

        private void Init_()
        {
            Config_.SetHideScrollbars(true);
            DevicePixelRatio = UnknownDevicePixelRatio_;
            WaitBeforeScreenshots = 100;
            DefaultMatchSettings.UseDom = false;
            ServerConnector.SdkName = FullAgentId;
            ServerConnector.AgentId = FullAgentId;
        }

        protected override Applitools.Configuration Configuration => configProvider_.GetConfiguration();

        protected Configuration Config_ => configProvider_.GetConfiguration();

        #endregion Constructors

        #region Properties

        public override string ServerUrl { get => base.ServerUrl ?? runner_.ServerUrl; set => base.ServerUrl = value; }

        /// <summary>
        /// Forces a full page screenshot (by scrolling and stitching) if the browser only 
        /// supports viewport screenshots).
        /// </summary>
        public bool ForceFullPageScreenshot { get => Config_.IsForceFullPageScreenshot ?? false; set => Config_.SetForceFullPageScreenshot(value); }
        public StitchModes StitchMode { get => Config_.StitchMode; set => Config_.SetStitchMode(value); }
        public int WaitBeforeScreenshots { get => Config_.WaitBeforeScreenshots; set => Config_.SetWaitBeforeScreenshots(value); }
        public bool HideScrollbars { get => Config_.HideScrollbars; set => Config_.SetHideScrollbars(value); }
        public bool HideCaret { get => Config_.HideCaret; set => Config_.SetHideCaret(value); }

        public override string ApiKey { get => base.ApiKey ?? runner_.ApiKey; set => base.ApiKey = value; }
        public override ProxySettings Proxy { get => base.Proxy ?? runner_.Proxy; set => base.Proxy = value; }

        protected bool? isDisabled_;
        protected object lastCheckSettings_;
        protected object lastCheckSettingsInternal_;
        protected IEyesJsExecutor jsExecutor_;

        public IDictionary<string, object> CachedSessionDetails { get; protected set; }

        public override bool IsDisabled { get => isDisabled_ ?? runner_.IsDisabled; set => isDisabled_ = value; }

        public IPositionProvider CurrentFramePositionProvider { get; protected set; }

        public void SetScrollToRegion(bool shouldScroll) { }

        public double DevicePixelRatio { get; set; }


        public string PlatformName { get; protected set; }

        #endregion Properties

        protected ScaleProviderFactory UpdateScalingParams()
        {
            ScaleProviderFactory factory;
            if (ScaleProvider is NullScaleProvider)
            {
                InitDevicePixelRatio_();

                Logger.Verbose("Setting scale provider...");
                try
                {
                    factory = GetScaleProviderFactory_();
                }
                catch (Exception)
                {
                    // This can happen in Appium for example.
                    Logger.Verbose("Failed to set ContextBasedScaleProvider. Using FixedScaleProvider instead...");
                    factory = new FixedScaleProviderFactory(1.0 / DevicePixelRatio, SetScaleProvider);
                }
                Logger.Verbose("Done!");
            }
            else
            {
                factory = new ScaleProviderIdentityFactory(ScaleProvider, (sp) => { });
            }

            return factory;
        }
       
        protected abstract ScaleProviderFactory GetScaleProviderFactory_();
        
        protected void InitDevicePixelRatio_()
        {
            Logger.Verbose("Trying to extract device pixel ratio...");
            try
            {
                object dpr = JSBrowserCommands.WithReturn.GetDevicePixelRatio(jsExecutor_);
                DevicePixelRatio = double.Parse(dpr.ToString());
            }
            catch (Exception)
            {
                Logger.Verbose("Failed to extract device pixel ratio! Using default.");
                DevicePixelRatio = DefaultDevicePixelRatio_;
            }
            Logger.Verbose("Device pixel ratio: {0}", DevicePixelRatio);
        }

        public void Check(ICheckSettings checkSettings)
        {
            throw new System.NotImplementedException();
        }

        public void Check(params ICheckSettings[] checkSettings)
        {
            throw new System.NotImplementedException();
        }

        protected override object GetAgentSetup()
        {
            Assembly eyesSeleniumAsm = this.GetType().Assembly;
            Assembly eyesCoreAsm = typeof(EyesBase).Assembly;
            string eyesSeleniumTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesSeleniumAsm);
            string eyesCoreTargetFramework = CommonUtils.GetAssemblyTargetFramework_(eyesCoreAsm);
            string fluentCommandString = null;
            if (lastCheckSettingsInternal_ is ICheckSettingsInternal csInternal)
            {
                fluentCommandString = csInternal.GetFluentCommandString();
            }
            else if (lastCheckSettingsInternal_ is IList<ICheckSettingsInternal> lstCsInternal)
            {
                StringBuilder fluentCommandSB = new StringBuilder();
                foreach (ICheckSettingsInternal csi in lstCsInternal)
                {
                    fluentCommandSB.Append(csi.GetFluentCommandString());
                    fluentCommandSB.Append(", ");
                }
                if (fluentCommandSB.Length > 2) fluentCommandSB.Length -= 2;
                fluentCommandString = fluentCommandSB.ToString();
            }
            object remoteWebDriver, capabilities;
            HtmlElementInfo html, body;
            string scrollRootElement, sessionId;
            GetWebDriverInfo(out remoteWebDriver, out html, out body, out scrollRootElement, out sessionId, out capabilities);
            return new
            {
                SeleniumSessionId = sessionId,
                WebDriver = new
                {
                    remoteWebDriver.GetType().Name,
                    remoteWebDriver.GetType().Assembly.GetName().Version,
                    Capabilities = capabilities,
                },
                SeleniumSDKAssemblyData = new
                {
                    eyesSeleniumAsm.ImageRuntimeVersion,
                    eyesSeleniumTargetFramework
                },
                CoreSDKAssemblyData = new
                {
                    eyesCoreAsm.ImageRuntimeVersion,
                    eyesCoreTargetFramework
                },
                FullAgentId,
                DevicePixelRatio,
                CutProvider,
                ScaleProvider,
                Config_.StitchMode,
                Config_.HideScrollbars,
                Config_.HideCaret,
                ForceFullPageScreenshot,
                HtmlSizes = new
                {
                    DocumentElement = new
                    {
                        html.Id,
                        Client = html.ClientSize,
                        Scroll = html.ScrollSize,
                        Reported = html.Size
                    },
                    Body = new
                    {
                        body.Id,
                        Client = body.ClientSize,
                        Scroll = body.ScrollSize,
                        Reported = body.Size
                    },
                },
                ScrollRootElement = scrollRootElement,
                CheckSettings = lastCheckSettings_,
                FluentCommandString = fluentCommandString,
            };
        }

        protected abstract void GetWebDriverInfo(out object remoteWebDriver,
            out HtmlElementInfo html, out HtmlElementInfo body,
            out string scrollRootElement, out string sessionId, out object capabilities);


        public IList<TestResultContainer> GetAllTestResults()
        {
            if (!((IEyes)this).IsCompleted())
            {
                return null;
            }

            return new TestResultContainer[] { testResultContainer_ };
        }

        public override IDictionary<string, IRunningTest> GetAllTests()
        {
            return new Dictionary<string, IRunningTest>() { { TestId, this } };
        }

        bool IEyes.IsCompleted()
        {
            return testResultContainer_ != null;
        }

        protected internal override MatchWindowData PrepareForMatch(ICheckTask checkTask)
        {
            throw new NotImplementedException();
        }

        protected internal override ICheckTask IssueCheck(ICheckSettings checkSettings,
            IList<VisualGridSelector[]> regionSelectors, string source, IList<IUserAction> userInputs)
        {
            throw new NotImplementedException();
        }

        protected internal override void CheckCompleted(ICheckTask checkTask, MatchResult matchResult)
        {
            throw new NotImplementedException();
        }

        protected override void SetConfigImpl(Applitools.IConfiguration configuration)
        {
            throw new NotImplementedException();
        }
        public abstract int GetFrameChainLength();


        public bool IsCompleted()
        {
            throw new System.NotImplementedException();
        }

        public abstract void AddKeyboardTrigger(IWebElement element, string text);
        public abstract void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor);
    }
}
