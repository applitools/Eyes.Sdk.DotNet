using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Fluent;
using Applitools.Playwright.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Playwright.Universal.Mapper;
using Applitools.Universal;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;
using IFrame = Applitools.Commands.IFrame;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright
{
    public class Eyes : EyesBase
    {
        private readonly PlaywrightEyesRunner runner_;

        /// <summary>
        /// The command executor to execute the commands.
        /// </summary>
        private readonly CommandExecutor commandExecutor_;

        /// <summary>
        /// The driver reference associated with the test.
        /// </summary>
        private Driver driver_;

        #region ctors

        /// <summary>
        /// Instantiates a new Eyes.
        /// </summary>
        public Eyes() : this(new ClassicRunner())
        {
        }

        public Eyes(ILogHandler logHandler = null)
            : this(new ClassicRunner(), logHandler: logHandler)
        {
        }

        public Eyes(Uri serverUrl, ILogHandler logHandler = null)
            : this(new ClassicRunner(), serverUrl?.AbsolutePath, logHandler)
        {
        }

        public Eyes(
            PlaywrightEyesRunner runner,
            string serverUrlFieldName = "remoteServerUri",
            ILogHandler logHandler = null)
            : base(runner, serverUrlFieldName: serverUrlFieldName, logHandler: logHandler)
        {
        }

        /// <summary>
        /// Instantiates a new Eyes.
        /// </summary>
        /// <param name="runner">The runner</param>
        public Eyes(PlaywrightEyesRunner runner)
            : base(runner)
        {
            runner_ = runner ?? new ClassicRunner();
            Runner = runner_;
            commandExecutor_ = Runner.CommandExecutor;
        }

        #endregion

        public Configuration GetConfiguration()
        {
            return Config;
        }

        /// <summary>
        /// Starts a test.
        /// </summary>
        /// <param name="page">The web driver that controls the browser hosting the application under test.</param>
        /// <returns>The Page</returns>
        public IPage Open(IPage page)
        {
            return Open(page, Config.AppName, Config.TestName);
        }

        public IPage Open(IPage page, string appName, string testName)
        {
            return Open(page, appName, testName, Config.ViewportSize);
        }

        public IPage Open(IPage page, string appName, string testName, RectangleSize viewportSize)
        {
            if (IsDisabled)
            {
                return page;
            }
            
            if (string.IsNullOrEmpty(ApiKey))
            {
                ApiKey = API_KEY;
            }
            
            driver_ = new Driver(page);
            Refer refer = GetRefer_();
            driver_.ApplitoolsRefId = refer.Ref(page, driver_.Root);
            driver_.Proxy = Proxy;
            Runner.ApiKey = ApiKey;
            Runner.ServerUrl = ServerUrl;
            Runner.Proxy = Proxy;

            Config.SetAppName(appName).SetTestName(testName);
            if (viewportSize != null && !viewportSize.IsEmpty())
            {
                Config.SetViewportSize(new RectangleSize(viewportSize));
            }

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(Runner.ManagerApplitoolsRefId);
            var openEyesResponsePayload =
                Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException($"{error.Message}{Environment.NewLine}{error.Stack}");
            }

            IsOpen = true;
            return page;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(string applitoolsRefId)
        {
            var result = new EyesManagerOpenEyes
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new OpenEyesRequestPayload
                {
                    Manager = new ManagerRef
                    {
                        ApplitoolsRefId = applitoolsRefId
                    },
                    Target = driver_,
                    Config = ToConfig(Config),
                    Settings = ToOpenSettings(Config)
                }
            };
            result.Payload.Config.Open.Properties = properties_; // ?
            return result;
        }

        /// <summary>
        /// Takes multiple screenshots at once (given all <see cref="ICheckSettings"/> objects are on the same level).
        /// </summary>
        /// <param name="checkSettings">Multiple <see cref="ICheckSettings"/> object representing different regions in the viewport</param>
        public void Check(params ICheckSettings[] checkSettings)
        {
            foreach (ICheckSettings cSs in checkSettings)
            {
                Check(cSs);
            }
        }

        public void Check(string tag, ICheckSettings checkSettings)
        {
            Check(checkSettings.WithName(tag));
        }

        public void Check(ICheckSettings checkSettings)
        {
            if (IsDisabled)
            {
                return;
            }

            ArgumentGuard.NotNull(checkSettings, nameof(checkSettings));
            ArgumentGuard.NotOfType(checkSettings, typeof(IPlaywrightCheckSettings), nameof(checkSettings));
            IPlaywrightCheckSettings playwrightCheckSettings = (IPlaywrightCheckSettings)checkSettings;

            CheckImpl_(playwrightCheckSettings);
        }

        protected override TestResults CloseImpl(bool throwEx, bool callGetResults)
        {
            var result = base.CloseImpl(throwEx, callGetResults);

            if (!IsOpen && driver_ != null)
            {
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;
            }

            return result;
        }

        protected override TestResults AbortImpl(bool callGetResults)
        {
            var result = base.AbortImpl(callGetResults);

            if (!IsOpen && driver_ != null)
            {
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;
            }

            return result;
        }

        public override ICollection<string> ExtractText(ICollection<OcrRegionBase> regions)
        {
            return ExtractTextImpl(target: driver_, regions);
        }
        
        public override IDictionary<string, IList<TextRegion>> ExtractTextRegions(TextRegionSettings textRegionSettings)
        {
            Logger.Log(nameof(ExtractTextRegions));

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, IList<TextRegion>>();
            }

            var eyesConfig = ToConfig(Config);
            var extractTextRegionsRequest = new EyesExtractTextRegionsRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new ExtractTextRegionsRequestPayload
                {
                    Target = driver_,
                    Config = eyesConfig,
                    Settings = textRegionSettings
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRegionsRequest, EyesExtractTextRegionsResponse>(extractTextRegionsRequest);

            return response.Payload.Result;
        }

        public override IDictionary<string, IList<Region>> Locate(VisualLocatorSettings settings)
        {
            return LocateImpl(driver_, settings);
        }

        protected override TRegion GetOcrRegion(OcrRegionBase ocrRegion)
        {
            if (!(ocrRegion is OcrRegion pwOcrRegion))
            {
                return null;
            }
            
            var selector = pwOcrRegion.GetSelector();
            if (selector != null)
            {
                return new Selector(selector);
            }

            var element = pwOcrRegion.GetElement();
            if (element != null)
            {
                return new Element(element)
                {
                    ApplitoolsRefId = GetRefer_().Ref(element, driver_)
                };
            }

            var reg = pwOcrRegion.GetRegion();
            if (reg == null)
            {
                throw new EyesException("can't handle uninitialized region.");
            }
            
            var region = reg.Value;
            return new RegionRectangle
            {
                X = region.Left,
                Y = region.Top,
                Height = region.Height,
                Width = region.Width
            };
        }
        
        private void CheckImpl_(ICheckSettings checkSettings)
        {
            Logger.Log("CheckImpl");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Check, StageType.Disabled);
                return;
            }

            var playwrightCheckSettings = (IPlaywrightCheckTarget)checkSettings;
            var eyesConfig = ToConfig(Config);
            var request = new EyesCheckRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new CheckRequestPayload
                {
                    Eyes = eyes_,
                    Config = eyesConfig,
                    Target = driver_,
                    Settings = CreateUniversalCheckSettings(playwrightCheckSettings)
                }
            };

            var result = commandExecutor_.SendRequest<EyesCheckRequest, EyesCheckResponse>(request);
            var error = result?.Payload?.Error;
            if (error != null)
            {
                throw new EyesException(error.ToString());
            }
        }

        protected override UniversalCheckSettings CreateUniversalCheckSettings(
            ICheckSettingsInternal checkSettingsInternal)
        {
            var playwrightCheckSettings = (IPlaywrightCheckTarget)checkSettingsInternal;
            var settings = base.CreateUniversalCheckSettings(checkSettingsInternal);
            settings.Frames = GetFrames(checkSettingsInternal);
            settings.ScrollRootElement =
                TRegionMapper.ToTRegionDtoFromSre(playwrightCheckSettings.GetScrollRootElement(), GetRefer_(),
                    driver_.Root);
            return settings;
        }

        protected override ICollection<CodedRegionReference> CollectRegions(IGetRegions[] regions)
        {
            return regions.Select(r => ((IPlaywrightReference<CodedRegionReference>)r).ToRegion(driver_, GetRefer_())).ToArray();
        }

        protected override ICollection<TFloatingRegion> CollectRegions(IGetFloatingRegion[] regions)
        {
            return regions.Select(r => ((IPlaywrightReference<TFloatingRegion>)r).ToRegion(driver_, GetRefer_())).ToArray();
        }

        protected override ICollection<TAccessibilityRegion> CollectRegions(IGetAccessibilityRegion[] regions)
        {
            return regions.Select(r => ((IPlaywrightReference<TAccessibilityRegion>)r).ToRegion(driver_, GetRefer_())).ToArray();
        }

        protected override TRegion GetUniversalRegion(ICheckSettingsInternal checkSettings)
        {
            TRegion region = null;
            if (checkSettings is ICheckSettingsInternal checkSettingsInternal)
            {
                region = CreateCheckSettingsTargetRegion(checkSettingsInternal);
            }

            if (region == null &&
                checkSettings is IPlaywrightCheckTarget playwrightCheckTarget)
            {
                Reference targetElement = playwrightCheckTarget.GetTargetElement();
                if (targetElement is Element element)
                {
                    targetElement.ApplitoolsRefId = GetRefer_().Ref(element.ElementHandle, driver_.Root);
                }
                else if (targetElement is Selector selector && selector.Locator != null)
                {
                    targetElement.ApplitoolsRefId = GetRefer_().Ref(selector.Locator, driver_.Root);
                }

                return targetElement;
            }

            return region;
        }

        private IList<IFrame> GetFrames(ICheckSettingsInternal checkSettings)
        {
            var frames = new List<IFrame>();
            if (!(checkSettings is IPlaywrightCheckTarget playwrightCheckSettings))
            {
                return frames;
            }

            frames = playwrightCheckSettings.GetFrameChain().Select(
                frame => (IFrame)TFramesMapper.ToContextReferenceDto(frame, GetRefer_(), driver_.Root)).ToList();

            return frames;
        }

        private Refer GetRefer_()
        {
            return runner_.Refer;
        }
    }
}