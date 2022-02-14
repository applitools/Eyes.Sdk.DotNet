using Applitools.Utils;
using System;
using System.Drawing;

namespace Applitools
{
    public class FileDebugScreenshotProvider : IDebugScreenshotProvider
    {
        private static readonly string DEFAULT_PREFIX = "screenshot_";
        private static readonly string DEFAULT_PATH = "";

        private string path_;
        private string prefix_;
        private Logger logger_;

        public FileDebugScreenshotProvider()
        {
            Path = DEFAULT_PATH;
            Prefix = DEFAULT_PREFIX;
        }

        public string Path
        {
            get { return path_; }
            set
            {
                path_ = value;

                if (string.IsNullOrEmpty(value))
                {
                    path_ = DEFAULT_PATH;
                }
                else
                {
                    path_ = value;
                    if (path_[path_.Length - 1] != System.IO.Path.DirectorySeparatorChar && path_[path_.Length - 1] != System.IO.Path.AltDirectorySeparatorChar)
                    {
                        path_ += System.IO.Path.DirectorySeparatorChar;
                    }
                }
            }
        }

        public string Prefix
        {
            get { return prefix_; }
            set { prefix_ = value ?? DEFAULT_PREFIX; }
        }

        public void SetLogger(Logger logger)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            logger_ = logger;
        }

        public void Save(Bitmap image, string suffix)
        {
            string filename = $"{Path}{prefix_}{DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff")}_{suffix}.png";
            try
            {
                logger_.Log(TraceLevel.Debug, Stage.Check, StageType.CaptureScreenshot, new { filename });
                image.Save(filename);
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.Check, StageType.CaptureScreenshot, e);
            }
        }
    }
}