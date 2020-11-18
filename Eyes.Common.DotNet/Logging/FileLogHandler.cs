using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Writes log messages to the standard output stream.
    /// </summary>
    [ComVisible(true)]
    public class FileLogHandler : LogHandlerBase
    {
        #region Constructors

        /// <summary>
        /// Creates a new <see cref="FileLogHandler"/> instance.
        /// </summary>
        /// <param name="filename">Where to write the log.</param>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        /// <param name="append">Whether to append to the file or create a new one.</param>
        public FileLogHandler(string filename, bool append, bool isVerbose)
            : base(isVerbose)
        {
            AppendToFile = append;
            FilePath = filename;
            Open();
        }

        /// <summary>
        /// Creates a new <see cref="FileLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        /// <param name="append">Whether to append to the file or create a new one.</param>
        public FileLogHandler(bool isVerbose, bool append = false)
            : this(Path.Combine(Environment.CurrentDirectory, "eyes.log"), append, isVerbose)
        {
        }

        /// <summary>
        /// Creates a new <see cref="FileLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public FileLogHandler()
            : this(true)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Whether to handle or ignore verbose log messages.
        /// </summary>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// Whether to append messages to the log file or to reset it on <see cref="Open"/>.
        /// </summary>
        public bool AppendToFile { get; set; }

        /// <summary>
        /// The path to the log file.
        /// </summary>
        public string FilePath { get; set; }

        #endregion

        #region Methods

        public override void OnMessage(string message, TraceLevel level)
        {
            try
            {
                File.AppendAllText(FilePath, message + Environment.NewLine);
            }
            catch
            {
                // We don't want a trace failure the fail the test
            }
        }

        public override void Open()
        {
            try
            {
                lock (FilePath)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                }
                if (!AppendToFile)
                {
                    lock (FilePath)
                    {
                        FileUtils.WriteTextFile(FilePath, string.Empty, false);
                    }
                }
            }
            catch
            {
                // We don't want a trace failure the fail the test
            }
        }

        #endregion
    }
}
