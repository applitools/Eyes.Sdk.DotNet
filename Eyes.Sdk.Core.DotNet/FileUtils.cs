namespace Applitools.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// File / folder related utilities.
    /// </summary>
    public static class FileUtils
    {
        #region Write

        /// <summary>
        /// Creates a UTF8 encoded text file containing the input text
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="text">Text to write</param>
        /// <param name="force">Whether to create missing directories</param>
        public static void WriteTextFile(string path, string text, bool force = false)
        {
            string dir = Path.GetDirectoryName(path);
            if (force)
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }

            if (Directory.Exists(dir))
            {
                using (var sw = File.CreateText(path))
                {
                    sw.Write(text);
                }
            }
        }


        /// <summary>
        /// Appends the specified text to the text file at the specified path.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="text">Text to append</param>
        /// <param name="newLine">Whether to add a new line at the end of the text</param>
        public static void AppendToTextFile(string path, string text, bool newLine = true)
        {
            ArgumentGuard.NotNull(path, nameof(path));
            ArgumentGuard.NotNull(text, nameof(text));

            if (newLine)
            {
                text += Environment.NewLine;
            }

            if (!File.Exists(path))
            {
                WriteTextFile(path, text, true);
                return;
            }

            using (var sw = File.AppendText(path))
            {
                sw.Write(text);
            }
        }

        /// <summary>
        /// Appends the specified text to the text file at the specified path.
        /// </summary>
        /// <param name="path">File path</param>
        /// <param name="lines">Text to append</param>
        public static void AppendToTextFile(string path, string[] lines)
        {
            ArgumentGuard.NotNull(path, nameof(path));
            ArgumentGuard.NotNull(lines, nameof(lines));

            StringBuilder sb = new StringBuilder(2000);
            foreach (string line in lines)
            {
                sb.Append(line).AppendLine();
            }

            if (!File.Exists(path))
            {
                WriteTextFile(path, sb.ToString(), true);
                return;
            }

            using (var sw = File.AppendText(path))
            {
                sw.Write(sb.ToString());
            }
        }

        /// <summary>
        /// Returns a stream optimized for sequentially writing to the input file.
        /// </summary>
        public static FileStream GetSequentialWriter(string file, int bufferSize = 8 * 1024)
        {
            return new FileStream(
                file,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                FileOptions.SequentialScan);
        }

        /// <summary>
        /// Returns a stream optimized for sequentially writing to the input file only if the file
        /// does not exist.
        /// </summary>
        /// <exception cref="IOException">File already exists</exception>
        public static FileStream GetSequentialWriterIfNew(string file, int bufferSize = 8 * 1024)
        {
            return new FileStream(
                file,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                FileOptions.SequentialScan);
        }

        #endregion

        #region Read

        /// <summary>
        /// Returns a stream optimized for sequentially reading the input existing file
        /// while allowing other readers.
        /// </summary>
        [DebuggerNonUserCode]
        public static FileStream GetSequentialReader(string file, int bufferSize = 8 * 1024)
        {
            return new FileStream(
                file,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                bufferSize,
                FileOptions.SequentialScan);
        }

        #endregion

        #region Create

        /// <summary>
        /// Creates all missing directories in the input paths.
        /// </summary>
        /// <returns>If <c>throwEx</c> is <c>false</c> return <c>false</c> on failure</returns>
        public static bool CreateDirectories(IEnumerable<string> paths, bool throwEx = false)
        {
            ArgumentGuard.NotNull(paths, nameof(paths));

            try
            {
                foreach (var path in paths)
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
            }
            catch
            {
                if (throwEx)
                {
                    throw;
                }

                return false;
            }

            return true;
        }

        #endregion

        #region Path

        /// <summary>
        /// Returns the full path to the folder containing the input assembly.
        /// </summary>
        public static string GetAssemblyFolder(Assembly assembly)
        {
            ArgumentGuard.NotNull(assembly, nameof(assembly));

            return Path.GetFullPath(Path.GetDirectoryName(assembly.Location));
        }

        /// <summary>
        /// Returns the full path the folder containing the calling assembly.
        /// </summary>
        public static string GetAssemblyFolder()
        {
            return GetAssemblyFolder(Assembly.GetCallingAssembly());
        }

        #endregion

        #region Download

        /// <summary>
        /// Downloads a file from the input URI and saves it in the specified path.
        /// </summary>
        /// <param name="uri">The input URI</param>
        /// <param name="path">The path to which to save the downloaded file</param>
        /// <param name="overwrite">Whether it is allowed to overwrite an existing file</param>
        public static void DownloadFile(Uri uri, string path, bool overwrite = true)
        {
            ArgumentGuard.NotNull(uri, nameof(uri));
            ArgumentGuard.NotNull(path, nameof(path));

            var request = HttpWebRequest.Create(uri);
            using (var res = (HttpWebResponse)request.GetResponse())
            using (var s = res.GetResponseStream())
            using (var sw = overwrite ?
                GetSequentialWriter(path) : GetSequentialWriterIfNew(path))
            {
                s.CopyTo(sw);
            }
        }

        #endregion
    }
}
