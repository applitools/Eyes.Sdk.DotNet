using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Applitools.Utils
{
    /// <summary>
    /// Represents a parsed User Agent string, and exposes its different component.
    /// </summary>
    public class UserAgent
    {
        private const string MajorMinor_ = @"(?<major>\d+)(?:[_.](?<minor>\d+))?";
        private const string Product_ = "(?:(?<product>{0})/" + MajorMinor_ + ")";

        private static readonly NameValueCollection NoHeaders_ =
            new NameValueCollection();

        private static readonly Regex VerRegex_ = new Regex(Product_.Fmt("Version"));

        private static readonly Regex BrowserRegex_ = new Regex(
            Product_.Fmt("Opera") + "|" +
            Product_.Fmt("Chrome") + "|" +
            Product_.Fmt("Safari") + "|" +
            Product_.Fmt("Firefox") + "|" +
            Product_.Fmt("Edge") + "|" +
            "(?:MS(?<product>IE) " + MajorMinor_ + ")");

        private static readonly Regex OSRegex_ = new Regex(
            @"(?:(?<os>Windows NT) " + MajorMinor_ + ")|" +
            @"(?:(?<os>Windows XP))|" +
            @"(?:(?<os>Windows 2000))|" +
            @"(?:(?<os>Windows NT))|" +
            @"(?:(?<os>Windows))|" +
            @"(?:(?<os>Mac OS X) " + MajorMinor_ + ")|" +
            @"(?:(?<os>Android) " + MajorMinor_ + ")|" +
            @"(?:(?<os>CPU(?: i[a-zA-Z]+)? OS) " + MajorMinor_ + ")|" +
            @"(?:(?<os>Mac OS X))|" +
            @"(?:(?<os>Mac_PowerPC))|" +
            @"(?:(?<os>Linux))|" +
            @"(?:(?<os>CrOS))|" +
            @"(?:(?<os>SymbOS))");

        private static readonly Regex HiddenIE_ = new Regex(
            @"(?:(?:rv:" + MajorMinor_ + "\\) like Gecko))");

        private static readonly Regex Edge_ = new Regex(Product_.Fmt("Edge"));

        public string OriginalUserAgentString { get; private set; }
        public string OS { get; private set; }
        public string OSMajorVersion { get; private set; }
        public string OSMinorVersion { get; private set; }
        public string Browser { get; private set; }
        public string BrowserMajorVersion { get; private set; }
        public string BrowserMinorVersion { get; private set; }

        /// <summary>
        /// Parses the input user-agent string.
        /// </summary>
        /// <param name="userAgent">User agent string to parse</param>
        /// <param name="unknowns">Whether to treat unknown products as <c>Unknown</c> or
        /// throw exceptions</param>
        public static UserAgent ParseUserAgentString(string userAgent, bool unknowns = true)
        {
            ArgumentGuard.NotNull(userAgent, nameof(userAgent));

            userAgent = userAgent.Trim();
            var result = new UserAgent();
            result.OriginalUserAgentString = userAgent;
            // OS
            var oss = new Dictionary<string, Match>();
            var matches = OSRegex_.Matches(userAgent);
            foreach (Match m in matches)
            {
                oss[m.Groups["os"].Value.ToLowerInvariant()] = m;
            }

            Match osmatch = null;
            if (matches.Count == 0)
            {
                if (unknowns)
                {
                    result.OS = OSNames.Unknown;
                }
                else
                {
                    throw new NotSupportedException("Unknown OS: {0}".Fmt(userAgent));
                }
            }
            else
            {
                if (oss.Count > 1 && oss.ContainsKey("android"))
                {
                    osmatch = oss["android"];
                }
                else
                {
                    osmatch = oss.First().Value;
                }

                result.OS = osmatch.Groups["os"].Value;
                result.OSMajorVersion = osmatch.Groups["major"].Value;
                result.OSMinorVersion = osmatch.Groups["minor"].Value;
                if (!string.IsNullOrWhiteSpace(result.OSMajorVersion) && string.IsNullOrWhiteSpace(result.OSMinorVersion))
                    result.OSMinorVersion = "0";
            }

            // OS Normalization
            if (result.OS.StartsWithOrdinal("CPU"))
            {
                result.OS = OSNames.IOS;
            }
            else if (result.OS == "Windows XP")
            {
                result.OS = OSNames.Windows;
                result.OSMajorVersion = "5";
                result.OSMinorVersion = "1";
            }
            else if (result.OS == "Windows 2000")
            {
                result.OS = OSNames.Windows;
                result.OSMajorVersion = "5";
                result.OSMinorVersion = "0";
            }
            else if (result.OS == "Windows NT")
            {
                result.OS = OSNames.Windows;
                if (result.OSMajorVersion == "6" && result.OSMinorVersion == "1")
                {
                    result.OSMajorVersion = "7";
                    result.OSMinorVersion = "0";
                }
                else if (string.IsNullOrWhiteSpace(result.OSMajorVersion))
                {
                    result.OSMajorVersion = "4";
                    result.OSMinorVersion = "0";
                }
            }
            else if (result.OS == "Mac_PowerPC")
            {
                result.OS = OSNames.Macintosh;
            }
            else if (result.OS == "CrOS")
            {
                result.OS = OSNames.ChromeOS;
            }

            // Browser
            var browserMatch = BrowserRegex_.Match(userAgent);
            result.Browser = browserMatch.Groups["product"].Value;
            result.BrowserMajorVersion = browserMatch.Groups["major"].Value;
            result.BrowserMinorVersion = browserMatch.Groups["minor"].Value;

            var browserOK = browserMatch.Success;
            if (result.OS == OSNames.Windows)
            {
                var edgeMatch = Edge_.Match(userAgent);
                if (edgeMatch.Success)
                {
                    result.Browser = BrowserNames.Edge;
                    result.BrowserMajorVersion = edgeMatch.Groups["major"].Value;
                    result.BrowserMinorVersion = edgeMatch.Groups["minor"].Value;
                }

                // IE11 and later is "hidden" on purpose. 
                // http://blogs.msdn.com/b/ieinternals/archive/2013/09/21/
                //   internet-explorer-11-user-agent-string-ua-string-sniffing-
                //   compatibility-with-gecko-webkit.aspx
                var iematch = HiddenIE_.Match(userAgent);
                if (iematch.Success)
                {
                    result.Browser = BrowserNames.IE;
                    result.BrowserMajorVersion = iematch.Groups["major"].Value;
                    result.BrowserMinorVersion = iematch.Groups["minor"].Value;

                    browserOK = true;
                }
            }

            if (!browserOK)
            {
                if (unknowns)
                {
                    result.Browser = "Unknown";
                }
                else
                {
                    throw new NotSupportedException("Unknown browser: {0}".Fmt(userAgent));
                }
            }

            // Explicit browser version (if available)
            var versionMatch = VerRegex_.Match(userAgent);
            if (versionMatch.Success)
            {
                result.BrowserMajorVersion = versionMatch.Groups["major"].Value;
                result.BrowserMinorVersion = versionMatch.Groups["minor"].Value;
            }

            return result;
        }
    }
}