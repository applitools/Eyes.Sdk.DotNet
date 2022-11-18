using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Applitools.Utils;
using AutoMapper;

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

    //    /// <summary>
    //    /// Applitools Eyes base class.
    //    /// </summary>
    [ComVisible(true)]
    public abstract class EyesBase : EyesBaseConfig//, IEyesBase, IBatchCloser
    {
        public override bool IsDisabled
        {
            get => Runner.IsDisabled;
            set => Runner.IsDisabled = value;
        }

        protected EyesBase(EyesRunner runner = null, Configuration configuration = null, ILogHandler logHandler = null) 
            : base(configuration, logHandler, runner)
        {
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

        protected Config ToConfig(Configuration configuration)
        {
            var config = new MapperConfiguration(cfg => {
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
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<OpenSettings>(configuration);

            return settings;
        }

        protected CloseSettings ToCloseSettings(Configuration configuration, bool throwEx)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.AddProfile<MapProfiler>();
            });
            var mapper = config.CreateMapper();

            var settings = mapper.Map<CloseSettings>(configuration);
            settings.ThrowErr = throwEx;

            return settings;
        }
    }
}
