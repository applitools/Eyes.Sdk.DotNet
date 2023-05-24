using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Fluent;
using Applitools.Utils;
using AutoMapper;
using Region = Applitools.Utils.Geometry.Region;

// ReSharper disable InconsistentNaming

namespace Applitools
{
    /// <summary>
    /// Extracts text in the specified language from the input image region.
    /// </summary>
    public delegate string[] GetTextHandler(string imageId, IList<Rectangle> regions, string language);

    /// <summary>
    /// Creates an image of the specified region of the application window and returns its id.
    /// </summary>
    /// <param name="region">Image region or an empty rectangle to create an image of 
    /// the entire window.</param>
    public delegate string CreateImageHandler(Rectangle region);

    /// <summary>
    /// Applitools Eyes base class.
    /// </summary>
    [ComVisible(true)]
    public abstract class EyesBase : EyesBaseConfig//, IEyesBase, IBatchCloser
    {
        protected readonly string API_KEY = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");
        protected PropertiesCollection properties_;
        protected readonly string serverUrlFieldName_;
        protected EyesRef eyes_;

        public override bool IsDisabled
        {
            get => Runner.IsDisabled;
            set => Runner.IsDisabled = value;
        }

        protected EyesBase(EyesRunner runner = null, 
            string serverUrlFieldName = "remoteServerUri", 
            Configuration configuration = null,
            ILogHandler logHandler = null) 
            : base(configuration, logHandler, runner)
        {
            properties_ = new PropertiesCollection();
            serverUrlFieldName_ = serverUrlFieldName;
        }

        /// <summary>
        /// Writes the input message to this agent's log. Used by Eyes.qfl.
        /// </summary>
        public void Log(string message)
        {
            ArgumentGuard.NotNull(message, nameof(message));

            Logger.Log(TraceLevel.Notice, Stage.General, new { message });
        }

        /// <summary>
        /// Throws an <see cref="EyesException"/> with the input message. Used by Eyes.qfl.
        /// </summary>
        public void Throw(string message)
        {
            Logger.Log(TraceLevel.Error, Stage.General, new { message });
            throw new EyesException(message);
        }

        public TestResults AbortIfNotClosed()
        {
            return Abort();
        }

        public virtual void AbortAsync()
        {
            Abort();
        }

        protected Config ToConfig(Configuration configuration)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var openConfig = mapper.Map<OpenConfig>(configuration);
            var screenshotConfig = mapper.Map<ScreenshotConfig>(configuration);
            var checkConfig = mapper.Map<CheckConfig>(configuration);
            var closeConfig = mapper.Map<CloseConfig>(configuration);

            return new Config(openConfig, screenshotConfig, checkConfig, closeConfig);
        }

        protected OpenSettings ToOpenSettings(Configuration configuration)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<OpenSettings>(configuration);

            return settings;
        }
     
        protected LocateSettings ToLocateSettings(Configuration configuration, ICollection<string> locatorNames)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<LocateSettings>(configuration);
            settings.LocatorNames = locatorNames;

            return settings;
        }

        protected CloseSettings ToCloseSettings(Configuration configuration, bool throwEx, bool? removeDuplicateTests)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<CloseSettings>(configuration);
            settings.ThrowErr = throwEx;
            settings.RemoveDuplicateTests = removeDuplicateTests;

            return settings;
        }

        protected ResultsSettings ToResultsSettings(Configuration configuration, bool throwEx, bool? removeDuplicateTests)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<ResultsSettings>(configuration);
            settings.ThrowErr = throwEx;
            settings.RemoveDuplicateTests = removeDuplicateTests;

            return settings;
        }

        protected static EGClientSettings ToEgClientSettings(Configuration configuration)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<EGClientSettings>(configuration);

            return settings;
        }
        
        protected virtual UniversalCheckSettings CreateUniversalCheckSettings(ICheckSettingsInternal checkSettingsInternal)
        {
            var settings = new UniversalCheckSettings
            {
                Hooks = new Hooks
                {
                    BeforeCaptureScreenshot = checkSettingsInternal.GetScriptHooks().FirstOrDefault(i => i.Key == CheckSettings.BEFORE_CAPTURE_SCREENSHOT).Value
                },
                LayoutBreakpoints = GetLayoutBreakpoints(checkSettingsInternal),
                MatchLevel = checkSettingsInternal.GetMatchLevel(),
                Name = checkSettingsInternal.GetName(),
                Region = GetUniversalRegion(checkSettingsInternal),
                Fully = checkSettingsInternal.GetStitchContent(),
                UserCommandId = checkSettingsInternal.GetVariationGroupId(),
                IgnoreDisplacements = checkSettingsInternal.GetIgnoreDisplacements(),
                SendDom = checkSettingsInternal.GetSendDom(),
                UseDom = checkSettingsInternal.GetUseDom(),
                EnablePatterns = checkSettingsInternal.GetEnablePatterns(),
                //Frames = GetFrames(checkSettingsInternal),
                IgnoreRegions = checkSettingsInternal.GetIgnoreRegions().Select(i => i.ToRegion()).ToArray(),
                StrictRegions = checkSettingsInternal.GetStrictRegions().Select(i => i.ToRegion()).ToArray(),
                LayoutRegions = checkSettingsInternal.GetLayoutRegions().Select(i => i.ToRegion()).ToArray(),
                ContentRegions = checkSettingsInternal.GetContentRegions().Select(i => i.ToRegion()).ToArray(),
                FloatingRegions = checkSettingsInternal.GetFloatingRegions().Select(i => i.ToRegion()).ToArray(),
                AccessibilityRegions = checkSettingsInternal.GetAccessibilityRegions().Select(i => i.ToRegion()).ToArray(),
                LazyLoad = checkSettingsInternal.GetLazyLoad()
            };

            return settings;
        }
        
        private object GetLayoutBreakpoints(ICheckSettingsInternal settings)
        {
            if (settings.GetLayoutBreakpoints() != null && settings.GetLayoutBreakpoints().Any())
            {
                return settings.GetLayoutBreakpoints();
            }

            return settings.GetLayoutBreakpointsEnabled();
        }

        private TRegion GetUniversalRegion(ICheckSettingsInternal checkSettings)
        {
            TRegion region = null;
            if (checkSettings != null)
            {
                region = CreateCheckSettingsTargetRegion(checkSettings);
            }
            
            return region;
        }

        private TRegion CreateCheckSettingsTargetRegion(ICheckSettingsInternal settings)
        {
            TRegion region = null;

            var rectangle = settings.GetTargetRegion();
            if (rectangle.HasValue)
            {
                region = new RegionRectangle
                {
                    X = rectangle.Value.X,
                    Y = rectangle.Value.Y,
                    Width = rectangle.Value.Width,
                    Height = rectangle.Value.Height
                };
            }

            return region;
        }

        public IDictionary<string, IList<Region>> Locate(VisualLocatorSettings settings)
        {
            Logger.Log("Locate");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, IList<Region>>();
            }

            var eyesConfig = ToConfig(Config);
            var eyesLocateRequest = new EyesLocateRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new EyesLocateRequestPayload
                {
                    Config = eyesConfig,
                    Target = new ImageTarget
                    {
                        Image = settings.GetImage()
                    },
                    Settings = ToLocateSettings(Config, settings.GetNames())
                }
            };

            var response = Runner.SendRequest<EyesLocateRequest, EyesLocateResponse>(eyesLocateRequest);

            return response.Payload.Result;
        }

        public ICollection<string> ExtractText(params OcrRegion[] regions)
        {
            return ExtractText(regions.ToList());
        }

        public ICollection<string> ExtractText(ICollection<OcrRegion> regions)
        {
            if (IsDisabled)
            {
                return new List<string>();
            }

            var firstRegion = regions.FirstOrDefault();
            if (firstRegion == null)
            {
                throw new Exception("At least one region is required");
            }

            var eyesConfig = ToConfig(Config);
            var eyesExtractTextRequest = new EyesExtractTextRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new ExtractTextRequestPayload
                {
                    Target = new ImageTarget
                    {
                        Image = firstRegion.GetImage()
                    },
                    Config = eyesConfig,
                    Settings = regions.Select(r => new ExtractTextSettings
                    {
                        Hint = r.GetHint(),
                        MinMatch = r.GetMinMatch(),
                        Language = r.GetLanguage(),
                        Region = GetRegion(r)
                    }).ToList()
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRequest, EyesExtractTextResponse>(eyesExtractTextRequest);

            return response.Payload.Result;
        }

        public IDictionary<string, ICollection<TextRegion>> ExtractTextRegions(TextRegionSettings textRegionSettings)
        {
            Logger.Log("ExtractTextRegions");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new Dictionary<string, ICollection<TextRegion>>();
            }

            var eyesConfig = ToConfig(Config);
            var extractTextRegionsRequest = new EyesExtractTextRegionsRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new ExtractTextRegionsRequestPayload
                {
                    Target = new ImageTarget
                    {
                        Image = textRegionSettings.Image
                    },
                    Config = eyesConfig,
                    Settings = textRegionSettings
                }
            };

            var response = Runner.SendRequest<EyesExtractTextRegionsRequest, EyesExtractTextRegionsResponse>(extractTextRegionsRequest);

            return response.Payload.Result;
        }

        public virtual TestResults GetResults(bool throwEx = true)
        {
            Logger.Log("GetResults");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            var request = CreateEyesGetResultsRequest(throwEx, Runner.RemoveDuplicateTests);

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

        public virtual TestResults Abort()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            Logger.Verbose("enter. Abort");

            if (IsOpen == false)
            {
                return new TestResults
                {
                    Runner = Runner
                };
            }

            var request = CreateEyesAbortRequest();

            var payload = Runner.SendRequest<EyesAbortRequest, EyesCloseAbortResponse>(request).Payload;
            
            if (payload.Error != null)
            {
                IsOpen = false;
                throw new EyesException(payload.Error.ToString());
            }

            return GetResults();
        }

        public virtual TestResults Close(bool throwEx = true)
        {
            Logger.Log("Close");

            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.Close, StageType.Disabled);
                return new TestResults();
            }

            var eyesCloseRequest = CreateEyesCloseRequest(throwEx);

            var response = Runner.SendRequest<EyesCloseRequest, EyesCloseAbortResponse>(eyesCloseRequest);

            var error = response.Payload.Error;
            if (error != null && throwEx)
            {
                IsOpen = false;
                throw new TestFailedException(error.ToString());
            }

            return GetResults(throwEx);
        }

        private EyesCloseRequest CreateEyesCloseRequest(bool throwEx = false)
        {
            var eyesCloseRequest = new EyesCloseRequest
            {
                Payload = new CloseResponsePayload
                {
                    Eyes = eyes_,
                    Settings = ToCloseSettings(Config, throwEx, Runner.RemoveDuplicateTests)
                }
            };
            return eyesCloseRequest;
        }

        private EyesGetResultsRequest CreateEyesGetResultsRequest(bool throwEx = false, bool? removeDuplicateTests = null)
        {
            var eyesCloseRequest = new EyesGetResultsRequest
            {
                Payload = new EyesGetResultsRequestPayload
                {
                    Eyes = eyes_,
                    Settings = ToResultsSettings(Config, throwEx, removeDuplicateTests)
                }
            };

            return eyesCloseRequest;
        }

        private EyesAbortRequest CreateEyesAbortRequest()
        {
            var request = new EyesAbortRequest
            {
                Payload = new AbortPayload
                {
                    Eyes = eyes_
                }
            };
            return request;
        }

        private RegionRectangle GetRegion(OcrRegion ocrRegion)
        {
            var region = ocrRegion.GetRegion();
            if (region == null)
            {
                return null;
            }

            return new RegionRectangle
            {
                Width = region.Value.Width,
                Height = region.Value.Height,
                X = region.Value.Left,
                Y = region.Value.Top,
            };
        }

        public void Log(string msg, params object[] args)
        {
            Logger.Log(TraceLevel.Notice, TestName, Stage.General, new { message = string.Format(msg, args) });
        }

        public void AddProperty(string name, string value)
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }

            properties_.Add(name, value);
        }

        public void ClearProperties()
        {
            if (IsDisabled)
            {
                Logger.Log(TraceLevel.Warn, TestName, Stage.General, StageType.Disabled);
                return;
            }

            properties_.Clear();
        }
    }
}
