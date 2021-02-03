using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Runtime.Versioning;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Microsoft.Win32;

namespace Applitools.Utils
{
    /// <summary>
    /// Common utilities
    /// </summary>
    public static class CommonUtils
    {
        #region Methods

        #region Bits

        /// <summary>
        /// Writes the big-endian byte representation of the input value to the target
        /// stream.
        /// </summary>
        public static void ToBytesBE(int value, Stream stream)
        {
            ArgumentGuard.NotNull(stream, nameof(stream));

            value = IPAddress.HostToNetworkOrder(value);
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the big-endian byte representation of the input value to the target
        /// stream.
        /// </summary>
        public static void ToBytesBE(short value, Stream stream)
        {
            ArgumentGuard.NotNull(stream, nameof(stream));

            value = IPAddress.HostToNetworkOrder(value);
            var bytes = BitConverter.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
        }

        #endregion

        #region Stream

        /// <summary>
        /// Copies up to the specified maximal number of bytes to the destination stream.
        /// </summary>
        /// <param name="bufferSize">Size of chunks read and written</param>
        /// <param name="destination">Destination stream</param>
        /// <param name="maxBytes">Maximal number of bytes to copy or <c>-1</c> to allow
        /// any number of bytes to be copied</param>
        /// <param name="source">Source stream</param>
        /// <exception cref="ArgumentException">Thrown if the source stream contains more than 
        /// <c>maxBytes</c> bytes</exception>
        public static void CopyUpTo(
            this Stream source, Stream destination, long maxBytes, int bufferSize = 1024)
        {
            ArgumentGuard.NotNull(source, nameof(source));
            ArgumentGuard.NotNull(destination, nameof(destination));
            ArgumentGuard.GreaterOrEqual(maxBytes, -1, nameof(maxBytes));

            if (maxBytes == -1)
            {
                maxBytes = long.MaxValue;
            }

            byte[] buffer = new byte[bufferSize];
            var totalRead = 0;
            int bytesRead;

            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalRead += bytesRead;
                if (totalRead > maxBytes)
                {
                    throw new ArgumentException(
                        "source stream contains more than {0} bytes".Fmt(maxBytes));
                }

                destination.Write(buffer, 0, bytesRead);
            }
        }

        /// <summary>
        /// Returns all the bytes of the input stream.
        /// </summary>
        public static byte[] ReadToEnd(this Stream stream, int bufferSize = 1024, int length = 0, long maxLength = 0,
            Logger logger = null)
        {
            ArgumentGuard.NotNull(stream, nameof(stream));

            using (var ms = length == 0 ? new MemoryStream() : new MemoryStream(length))
            {
                byte[] array = new byte[bufferSize];
                int count;
                long maxPosition = maxLength - bufferSize;
                bool stillReading = true;
                NetworkStream networkStream = stream as NetworkStream;
                logger?.Log(TraceLevel.Debug, null, Stage.General, 
                    new { message = "start reading", streamType = stream.GetType().Name });
                while (stillReading && (networkStream?.DataAvailable ?? true) && (count = stream.Read(array, 0, array.Length)) != 0)
                {
                    if (maxLength > 0 && ms.Position >= maxPosition)
                    {
                        stillReading = false;
                        count = (int)Math.Max(0, maxLength - ms.Position);
                    }
                    ms.Write(array, 0, count);
                }
                byte[] bytes = ms.ToArray();
                logger?.Log(TraceLevel.Debug, null, Stage.General,
                    new { message = "done reading", streamType = stream.GetType().Name , bytesRead = bytes.Length});

                return bytes;
            }
        }

        #endregion

        #region Retry

        /// <summary>
        /// Runs the input <c>action</c> upon each expiry of <c>interval</c> until <c>action</c>
        /// returns <c>true</c> or <c>timeout</c> expires.
        /// </summary>
        /// <returns>Returns <c>true</c> if and only if <c>action</c> returned <c>true</c>
        /// </returns>
        public static bool Retry(TimeSpan timeout, TimeSpan interval, Func<bool> action)
        {
            ArgumentGuard.NotNull(action, nameof(action));

            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    if (action())
                    {
                        return true;
                    }
                }
                catch
                {
                }

                if (sw.Elapsed > timeout)
                {
                    return false;
                }

                Thread.Sleep(interval);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes of the input object unless it is null.
        /// </summary>
        public static void DisposeIfNotNull(this IDisposable disposable)
        {
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        #region Exceptions

        /// <summary>
        /// Executes the input action and returns an exception if one was thrown.
        /// </summary>
        /// <returns>The exception thrown or <c>null</c> if no exception was thrown.</returns>
        public static Exception DontThrow(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        public static string GetDotNetVersion()
        {
#if NET45
            return ".NET Framework 4.5";
#elif NET451
            return ".NET Framework 4.5.1";
#elif NET452
            return ".NET Framework 4.5.2";
#elif NET46
            return ".NET Framework 4.6";
#elif NET461
            return ".NET Framework 4.6.1";
#elif NET462
            return ".NET Framework 4.6.2";
#elif NET47
            return ".NET Framework 4.7";
#elif NET471
            return ".NET Framework 4.7.1";
#elif NET472
            return ".NET Framework 4.7.2";
#elif NET48
            return ".NET Framework 4.8";
#elif NETCOREAPP3_1
            return ".NET Core 3.1";
#elif NETCOREAPP3_0
            return ".NET Core 3.0";
#elif NETCOREAPP2_1 
            return ".NET Core 2.1";
#elif NETSTANDARD2_0
            return ".NET Core 2.0";
#else
            return ".NET " + Environment.Version.ToString();
#endif
        }
        #endregion

        #region Hash
        public static string GetSha256Hash(this byte[] bytes)
        {
            using (SHA256 sha256Alg = SHA256.Create())
            {
                return BytesToHexString(sha256Alg.ComputeHash(bytes));
            }
        }

        public static string BytesToHexString(byte[] array)
        {
            StringBuilder sb = new StringBuilder(array.Length * 2);
            for (int i = 0; i < array.Length; i++)
            {
                sb.Append($"{array[i]:X2}");
            }
            return sb.ToString();
        }

        #endregion

        #region Resources

        public static Stream ReadResourceStream(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            return thisAssembly.GetManifestResourceStream(filename);
        }

        public static string ReadResourceFile(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            using (Stream stream = thisAssembly.GetManifestResourceStream(filename))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static IList<string> ReadResourceFileAsLines(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            List<string> lines = new List<string>();
            using (Stream stream = thisAssembly.GetManifestResourceStream(filename))
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public static IList<string> ReadStreamAsLines(Stream stream)
        {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(stream))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            return lines;
        }

        public static byte[] ReadResourceBytes(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();

            using (Stream stream = thisAssembly.GetManifestResourceStream(filename))
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        #endregion

        public static string GetAssemblyTargetFramework_(Assembly eyesSeleniumAsm)
        {
            string targetFramework = null;
            object[] customAttributes = eyesSeleniumAsm.GetCustomAttributes(false);
            foreach (Attribute attr in customAttributes)
            {
                if (attr is TargetFrameworkAttribute targetFrameworkAttribute)
                {
                    targetFramework = targetFrameworkAttribute.FrameworkDisplayName;
                    if (string.IsNullOrWhiteSpace(targetFramework))
                    {
                        targetFramework = targetFrameworkAttribute.FrameworkName;
                    }
                    break;
                }
            }
            return targetFramework;
        }

        #endregion
        public static TAttribute GetAttribute<TAttribute>(this Enum value) where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static string GetEnvVar(string envVarName)
        {
            return Environment.GetEnvironmentVariable(envVarName) ?? Environment.GetEnvironmentVariable("bamboo_" + envVarName);
        }

        public static string SanitizeUrl(string url)
        {
            int hashIndex = url.LastIndexOf('#');
            string sanitizedUrl = (hashIndex >= 0 ? url.Substring(0, hashIndex) : url).TrimEnd('?');
            return sanitizedUrl;
        }

        public static string ServerUrl => GetEnvVar("APPLITOOLS_SERVER_URL") ?? CommonData.DefaultServerUrl;

        private static bool? dontCloseBatches_ = null;
        public static bool DontCloseBatches
        {
            get => dontCloseBatches_ ?? "true".Equals(GetEnvVar("APPLITOOLS_DONT_CLOSE_BATCHES"), StringComparison.OrdinalIgnoreCase);
            internal set => dontCloseBatches_ = value;
        }

        public static void LogExceptionStackTrace(Logger logger, Stage stage, Exception ex, params string[] testIds)
        {
            LogExceptionStackTrace(logger, stage, null, ex, testIds);
        }

        public static void LogExceptionStackTrace(Logger logger, Stage stage, StageType? type, Exception ex, params string[] testIds)
        {
            HashSet<string> ids = new HashSet<string>();
            if (testIds != null && testIds.Length > 0)
            {
                ids.UnionWith(testIds);
            }
            LogExceptionStackTrace(logger, stage, type, ex, ids);
        }

        public static void LogExceptionStackTrace(Logger logger, Stage stage, StageType? type, Exception ex, HashSet<string> testIds)
        {
            try
            {
                Console.Error.WriteLine(ex);
                logger.Log(TraceLevel.Error, testIds, stage, type, new { message = ex.ToString(), ex.StackTrace });
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e);
            }
        }

    }
}
