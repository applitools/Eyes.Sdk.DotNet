using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Playwright.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Playwright.Universal.Mapper;
using Applitools.Universal.Dto;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Microsoft.Playwright;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright
{
    public class Eyes : EyesBase
    {
        private PlaywrightEyesRunner runner_;
        
        /// <summary>
        /// The command executor to execute the commands.
        /// </summary>
        private readonly CommandExecutor commandExecutor_;

        /// <summary>
        /// Is eyes closed, used to check abort after close.
        /// </summary>
        private bool isClosed_;

        /// <summary>
        /// The driver reference associated with the test.
        /// </summary>
        private Driver driver_;

        private EyesRef eyes_;

        private readonly IOcrRegionTransformer ocrRegionTransformer_;

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
            ocrRegionTransformer_ = new OcrRegionTransformer();
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

            // ConfigurationDto configurationDto = ConfigurationMapper
            //     .toConfigurationDto(configuration_, Runner.isDontCloseBatches());
            // OpenSettingsDto settingsDto =
            //     SettingsMapper.toOpenSettingsDto(configuration_, Runner.isDontCloseBatches());

            var eyesManagerOpenEyes = CreateEyesManagerOpenEyes(Runner.ManagerApplitoolsRefId, driver_);
            var openEyesResponsePayload =
                Runner.SendRequest<EyesManagerOpenEyes, EyesManagerOpenEyesResponse>(eyesManagerOpenEyes).Payload;
            eyes_ = openEyesResponsePayload.Result;

            var error = openEyesResponsePayload.Error;
            if (error != null)
            {
                throw new EyesException($"{error.Message}{Environment.NewLine}{error.Stack}");
            }

            // eyesRef_ = commandExecutor_.ManagerOpenEyes(Runner.ManagerApplitoolsRefId, driver_, settingsDto,
            //     configurationDto);
            IsOpen = true;
            return page;
        }

        private EyesManagerOpenEyes CreateEyesManagerOpenEyes(
            string applitoolsRefId,
            Driver driver)
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
                    Target = new DriverTarget(),
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

            CheckSettingsDto checkSettingsDto = PlaywrightCheckSettingsMapper.ToCheckSettingsDto(
                playwrightCheckSettings,
                Config, GetRefer_(), driver_.Root);

            CheckDto_(checkSettingsDto, driver_);
        }

        public ICollection<string> ExtractText(ICollection<OcrRegion> ocrRegions)
        {
            Logger.Log("ExtractText");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new List<string>();
            }

            return new List<string>();
            //
            // var eyesConfig = ToConfig(Config);
            // eyesConfig.Open.Properties = properties_;
            // var serverUrl = GetServerUrl(CommandExecutor);
            // var eyesExtractTextRequest = new EyesExtractTextRequest
            // {
            //     Payload = new ExtractTextRequestPayload
            //     {
            //         Config = eyesConfig,
            //         Target = new DriverTarget
            //         {
            //             ServerUrl = serverUrl,
            //             Capabilities = GetCapabilities(remoteWebDriver),
            //             SessionId = remoteWebDriver.SessionId.ToString(),
            //             Proxy = Config.Proxy
            //         },
            //         Settings = ToExtractTextSettings(regions)
            //     }
            // };

            // var response = Runner.SendRequest<EyesExtractTextRequest, EyesExtractTextResponse>(eyesExtractTextRequest);

            // return response.Payload.Result;
        }

        private ICollection<ExtractTextSettings> ToExtractTextSettings(ICollection<OcrRegion> regions)
        {
            var extractTextSettings = new List<ExtractTextSettings>();
            foreach (var region in regions)
            {
                var ocrRegion = ocrRegionTransformer_.GetRegion(region);
                var extractTextSetting = new ExtractTextSettings
                {
                    Region = ocrRegion,
                    Hint = region.GetHint(),
                    Language = region.GetLanguage(),
                    MinMatch = region.GetMinMatch()
                };
                extractTextSettings.Add(extractTextSetting);
            }

            return extractTextSettings;
        }

        /// <summary>Locate</summary>
        /// <param name="visualLocatorSettings">The locator settings.</param>
        /// <returns>Mapping of the region locations.</returns>
        public Dictionary<string, List<Region>> Locate(VisualLocatorSettings visualLocatorSettings)
        {
            ArgumentGuard.NotNull(visualLocatorSettings, nameof(visualLocatorSettings));

            if (!IsOpen)
            {
                Abort();
                throw new EyesException("Eyes not open");
            }

            //
            // VisualLocatorSettingsDto visualLocatorSettingsDto = VisualLocatorSettingsMapper
            //     .toVisualLocatorSettingsDto(visualLocatorSettings);
            //
            // ConfigurationDto configurationDto = ConfigurationMapper
            //     .toConfigurationDto(Configure(), false);
            //
            // return LocateDto_(driver_, visualLocatorSettingsDto, configurationDto);
            return new Dictionary<string, List<Region>>();
        }

        /// <summary>
        /// Close test results.
        /// </summary>
        /// <param name="shouldThrowException">Throw exception if a diff is found</param>
        /// <returns>The test results</returns>
        public override TestResults Close(bool shouldThrowException = true)
        {
            if (IsDisabled)
            {
                return null;
            }

            if (!IsOpen)
            {
                throw new EyesException("Eyes not open");
            }

            // ConfigurationDto configurationDto = ConfigurationMapper
            //     .toConfigurationDto(configuration_, Runner.isDontCloseBatches());
            // CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
            //
            // commandExecutor_.Close(eyesRef_, settings, configurationDto);
            // GetResultsSettings getResultsSettings = new GetResultsSettings(shouldThrowException);
            // List<CommandEyesGetResultsResponseDto> getResultsResponse =
            //     commandExecutor_.EyesGetResults(eyesRef_, getResultsSettings);
            //
            // eyesRef_ = null;
            // isClosed_ = true;
            // IsOpen = false;
            // GetRefer_().Destroy(driver_.Root);
            // driver_ = null;
            //
            // List<TestResults> testResults =
            //     TestResultsMapper.toTestResultsList(getResultsResponse, getApiKey(), getServerUrl(), getProxy());
            // if (testResults == null)
            //     return null;
            // testResults.forEach(
            //     testResults1->runner.logSessionResultsAndThrowException(shouldThrowException, testResults1));
            // return testResults.get(0);
            return null;
        }

        public override TestResults GetResults(bool throwEx = true)
        {
            Logger.Log("GetResults");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            var request = new EyesGetResultsRequest
            {
                Payload = new EyesGetResultsRequestPayload
                {
                    Eyes = eyes_,
                    Settings = ToResultsSettings(Config, throwEx, Runner.RemoveDuplicateTests)
                }
            };

            var response = Runner.SendRequest<EyesGetResultsRequest, EyesGetResultsResponse>(request);

            var error = response.Payload.Error;
            if (error != null)
            {
                IsOpen = false;
                throw new TestFailedException(error.ToString());
            }

            var testResults = response.Payload.Result;

            foreach (var testResult in testResults)
            {
                testResult.Runner = Runner;
                if (testResult.IsPassed == false && throwEx)
                {
                    IsOpen = false;
                    throw new TestFailedException(testResult, $"{testResult.Url}{Environment.NewLine}");
                }
            }

            IsOpen = false;
            return testResults.First();
        }

        public void CloseAsync()
        {
            if (IsDisabled)
            {
                return;
            }

            if (!IsOpen)
            {
                throw new EyesException("Eyes not open");
            }

            // ConfigurationDto configurationDto = ConfigurationMapper
            //     .toConfigurationDto(configuration_, Runner.isDontCloseBatches());
            // CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
            //
            // commandExecutor_.Close(eyesRef_, settings, configurationDto);
            // eyesRef_ = null;
            isClosed_ = true;
            IsOpen = false;
            GetRefer_().Destroy(driver_.Root);
            driver_ = null;
        }

        public override void AbortAsync()
        {
            if (!isClosed_ && IsOpen)
            {
                // CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
                // commandExecutor_.Abort(eyesRef_, settings);
                //
                // eyesRef_ = null;
                IsOpen = false;
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;
            }
        }

        public override TestResults Abort()
        {
            if (!isClosed_ && IsOpen)
            {
                // CloseSettingsDto settings = SettingsMapper.toCloseSettingsDto(getConfiguration());
                // commandExecutor_.Abort(eyesRef_, settings);
                //
                // GetResultsSettings getResultsSettings = new GetResultsSettings(false);
                // List<CommandEyesGetResultsResponseDto> getResultsResponse =
                //     commandExecutor_.EyesGetResults(eyesRef_, getResultsSettings);
                //
                // eyesRef_ = null;
                IsOpen = false;
                GetRefer_().Destroy(driver_.Root);
                driver_ = null;

                // List<TestResults> testResults =
                //     TestResultsMapper.toTestResultsList(getResultsResponse, getApiKey(), getServerUrl(), getProxy());
                // if (testResults != null)
                // {
                //     return testResults.FirstOrDefault();
                // }
            }

            return null;
        }

        internal virtual void CheckImpl_(ICheckSettings checkSettings)
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
                    Target = new DriverTarget(),
                    // {
                    //     ServerUrl = serverUrl,
                    //     Capabilities = GetCapabilities(remoteWebDriver),
                    //     SessionId = remoteWebDriver.SessionId.ToString(),
                    //     Proxy = Config.Proxy
                    // },
                    //Settings = CreateUniversalCheckSettings(playwrightCheckSettings)
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

        private void CheckDto_(CheckSettingsDto checkSettingsDto, Driver driverTargetDto)
        {
            if (IsDisabled)
            {
                return;
            }

            if (!IsOpen)
            {
                Abort();
                throw new EyesException("you must call open() before checking");
            }

            ArgumentGuard.NotNull(checkSettingsDto, "checkSettings");
            //
            // ConfigurationDto configurationDto = ConfigurationMapper
            //     .toConfigurationDto(configuration, Runner.DontCloseBatches);
            // commandExecutor_.EyesCheck(eyesRef, driverTargetDto, checkSettingsDto, configurationDto);
        }

        // private List<string> ExtractTextDto_(Driver target, List<OCRExtractSettingsDto> settings,
        //     ConfigurationDto config)
        // {
        //     return commandExecutor_.ExtractText(target, settings, config);
        // }

        // private Dictionary<string, List<TextRegion>> LocateTextDto_(Driver target, OCRSearchSettingsDto settings,
        //     ConfigurationDto config)
        // {
        //     return commandExecutor_.LocateText(target, settings, config);
        // }

        // private Dictionary<string, List<Region>> LocateDto_(Driver target, VisualLocatorSettingsDto settings,
        //     ConfigurationDto config)
        // {
        //     Dictionary<string, List<RegionDto>> response = commandExecutor_.Locate(target, settings, config);
        //     return LocateMapper.toLocateFromDto(response);
        // }

        private Refer GetRefer_()
        {
            return runner_.Refer;
        }
    }
}