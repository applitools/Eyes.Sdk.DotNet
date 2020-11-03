using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Writes log messages to the standard output stream.
    /// </summary>
    [ComVisible(true)]
    public class FileLogHandler : LogHandlerBase
    {
        private readonly Queue<string> queue_ = new Queue<string>(100);
        private readonly AutoResetEvent continueWritingWaitHandle_ = new AutoResetEvent(false);
        private readonly AutoResetEvent writingDoneWaitHandle_ = new AutoResetEvent(false);
        private Thread fileWriterThread_;
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

        private void DumpLogToFile_()
        {
            while (IsOpen)
            {
                continueWritingWaitHandle_.WaitOne(2000);
                if (FilePath == null) continue;
                if (queue_.Count > 0)
                {
                    string[] logLines;
                    lock (queue_)
                    {
                        logLines = queue_.ToArray();
                        queue_.Clear();
                    }
                    lock (FilePath)
                    {
                        FileUtils.AppendToTextFile(FilePath, logLines);
                    }
                    Thread.Sleep(1000);
                }
                writingDoneWaitHandle_.Set();
            }
        }

        public override void OnMessage(string message, TraceLevel level)
        {
            try
            {
                lock (queue_)
                {
                    queue_.Enqueue(message);
                    continueWritingWaitHandle_.Set();
                }

                if (!isOpen_ && queue_.Count > 0)
                {
                    Open();
                    Close();
                }
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
                if (!AppendToFile)
                {
                    lock (FilePath)
                    {
                        FileUtils.WriteTextFile(FilePath, string.Empty, false);
                    }
                }
                if (fileWriterThread_ == null || !fileWriterThread_.IsAlive)
                {
                    //OnMessage("FileLogHandler: starting new thread", TraceLevel.Info);
                    fileWriterThread_ = new Thread(new ThreadStart(DumpLogToFile_));
                    fileWriterThread_.IsBackground = true;
                    isOpen_ = true;
                }
                fileWriterThread_.Start();
            }
            catch
            {
                // We don't want a trace failure the fail the test
            }
        }

        public override void Close()
        {
            //OnMessage("FileLogHandler: closing file", TraceLevel.Info);
            writingDoneWaitHandle_.Reset();
            continueWritingWaitHandle_.Set();
            writingDoneWaitHandle_.WaitOne(5000);
            isOpen_ = false;
        }

        #endregion
    }
}
