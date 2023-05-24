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
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;
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

            driver_ = new Driver(page);
            Refer refer = GetRefer_();
            driver_.ApplitoolsRefId = refer.Ref(page, driver_.Root);
            driver_.SetWebDriverProxy(Config.Proxy);

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

        public override TestResults Close(bool throwEx = true)
        {
            var result = base.Close(throwEx);
            
            if (!IsOpen)
            {
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;
            }
            
            return result;
        }

        public override TestResults Abort()
        {
            var result = base.Abort();
            
            if (!IsOpen)
            {
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;
            }

            return result;
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
            eyesConfig.Open.Properties = properties_;
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

            if (result?.Payload?.Result == null)
            {
                return;
            }

            foreach (var matchResult in result.Payload.Result)
            {
                if (matchResult.AsExpected)
                {
                    continue;
                }

                throw new EyesException($"Unexpected match result for {matchResult}");
            }
        }
  
        protected override UniversalCheckSettings CreateUniversalCheckSettings(ICheckSettingsInternal checkSettingsInternal)
        {
            var playwrightCheckSettings = (IPlaywrightCheckTarget)checkSettingsInternal;
            var settings = base.CreateUniversalCheckSettings(checkSettingsInternal);
            //settings.Frames = TFramesMapper.ToTFramesFromCheckSettings(playwrightCheckSettings.GetFrameChain(), GetRefer_(), driver_.Root);
            settings.ScrollRootElement = TRegionMapper.ToTRegionDtoFromSRE(playwrightCheckSettings.GetScrollRootElement(), GetRefer_(), driver_.Root);
            return settings;
        }

        private IList<Commands.IFrame> GetFrames(ICheckSettingsInternal checkSettings)
        {
            var frames = new List<Commands.IFrame>();
            if (checkSettings is IPlaywrightCheckTarget playwrightCheckSettings)
            {
                foreach (var item in playwrightCheckSettings.GetFrameChain())
                {
                    if (string.IsNullOrEmpty(item.FrameNameOrId) == false)
                    {
                        frames.Add(new FrameNameOrId
                        {
                            Frame = item.FrameNameOrId
                        });
                    }

                    if (item.FrameSelector != null)
                    {
                        frames.Add(new FrameSelector
                        {
                            //Frame = GetRegionSelector_(item.FrameSelector)
                        });
                    }

                    if (item.FrameElement != null)
                    {
                        frames.Add(new FrameElement
                        {
                            ElementId = item.FrameElement.ApplitoolsRefId
                        });
                    }
                }
            }

            return frames;
        }

        private Refer GetRefer_()
        {
            return runner_.Refer;
        }
    }
}