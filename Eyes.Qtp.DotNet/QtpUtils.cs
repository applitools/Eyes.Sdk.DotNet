namespace Applitools.Qtp
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Text.RegularExpressions;
    using System.Threading;
    using Utils;

    /// <summary>
    /// Eyes QTP utilities
    /// </summary>
    public class QtpUtils
    {
        #region Fields

        private const string MajorMinor_ = @"(?<major>[^ .;_)]+)(?:[_.](?<minor>[^ .;_)]+))?";
        private const string Product_ = "(?:(?<name>{0})[ ]+" + MajorMinor_ + ")";
        private const string BrowserIE_ = "internet explorer";
        private const string BrowserChrome_ = "Chrome";
        private const string BrowserEdge_ = "Edge";
        private const string BrowserFirefox_ = "Firefox";
        private const string BrowserSafari_ = "Safari";

        private static readonly Regex BrowserRegex_ = new Regex(
            Product_.Fmt(BrowserIE_) + "|" +
            Product_.Fmt(BrowserChrome_) + "|" +
            Product_.Fmt(BrowserEdge_) + "|" +
            Product_.Fmt(BrowserSafari_) + "|" +
            Product_.Fmt(BrowserFirefox_),
            RegexOptions.IgnoreCase);

        #endregion

        #region Methods

        /// <summary>
        /// Returns the name of the process of the input id.
        /// </summary>
        public string GetProcessName(int processId)
        {
            return SystemUtils.GetProcessName(processId);
        }

        /// <summary>
        /// Sleeps the specified amount of milliseconds.
        /// </summary>
        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Returns a representation of the current time at millisecond resolution.
        /// </summary>
        public long GetCurrentTime()
        {
            return (long)DateTimeOffset.Now.Subtract(DateTimeOffset.MinValue).TotalMilliseconds;
        }

        /// <summary>
        /// Returns the size of the image stored in the input path.
        /// </summary>
        public Size GetImageSize(string path)
        {
            ArgumentGuard.NotNull(path, nameof(path));

            using (var bmp = new Bitmap(path))
            {
                return bmp.Size;
            }
        }

        /// <summary>
        /// Downloads a file from the input url and saves it at the specified path.
        /// </summary>
        public void DownloadFile(string url, string path, bool overwrite = true)
        {
            FileUtils.DownloadFile(new Uri(url), path, overwrite);
        }

        /// <summary>
        /// Returns a normalized browser name and version given the input 
        /// <c>application version</c> property value.
        /// </summary>
        public string GetBrowserName(string appVersion)
        {
            var browserMatch = BrowserRegex_.Match(appVersion);
            if (!browserMatch.Success)
            {
                return appVersion;
            }

            var name = browserMatch.Groups["name"].Value;
            if (name.Equals(BrowserIE_, StringComparison.InvariantCultureIgnoreCase))
            {
                return "IE " + browserMatch.Groups["major"].Value;
            }

            if (name.Equals(BrowserFirefox_, StringComparison.InvariantCultureIgnoreCase))
            {
                return "Firefox";
            }

            if (name.Equals(BrowserSafari_, StringComparison.InvariantCultureIgnoreCase))
            {
                return "Safari";
            }

            if (name.Equals(BrowserChrome_, StringComparison.InvariantCultureIgnoreCase))
            {
                return "Chrome";
            }

            if (name.Equals(BrowserEdge_, StringComparison.InvariantCultureIgnoreCase))
            {
                return "Edge";
            }
            return appVersion;
        }

        private void TestGetBrowserName_()
        {
            var assert = new Dictionary<string, string>();
            assert["IE 10"] = GetBrowserName("internet explorer 10");
            assert["IE 11"] = GetBrowserName("Internet Explorer 11.0");
            assert["Firefox"] = GetBrowserName("Mozilla Firefox 27.0.1");
            assert["Chrome"] = GetBrowserName("Chrome 35.0");

            foreach (var p in assert)
            {
                if (p.Key != p.Value)
                {
                    throw new Exception("{0} != {1}".Fmt(p.Key, p.Value));
                }
            }
        }

        #endregion
    }
}
