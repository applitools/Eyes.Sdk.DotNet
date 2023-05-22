using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using Applitools.Fluent;
using Applitools.Utils;
using AutoMapper;

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

        public abstract TestResults Abort();
        public abstract TestResults Close(bool throwEx = true);
        public abstract TestResults GetResults(bool throwEx = true);

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
    }
}
