using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;

namespace Applitools
{
    [TestFixture]
    public class TestUserAgentParsing : ReportingTestSuite
    {
        [TestCase("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36", OSNames.Windows, "10", "0", BrowserNames.Chrome, "75", "0")]
        [TestCase("Mozilla/5.0 (Linux; Android 9; Android SDK built for x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.105 Mobile Safari/537.36", OSNames.Android, "9", "0", BrowserNames.Chrome, "72", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0", OSNames.Windows, "7", "0", BrowserNames.Firefox, "54", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko", OSNames.Windows, "7", "0", BrowserNames.IE, "11", "0")]
        [TestCase("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)", OSNames.Windows, "7", "0", BrowserNames.IE, "10", "0")]
        [TestCase("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/74.0.3729.157 Safari/537.36", OSNames.Linux, "", "", BrowserNames.Chrome, "74", "0")]
        [TestCase("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:50.0) Gecko/20100101 Firefox/50.0", OSNames.Linux, "", "", BrowserNames.Firefox, "50", "0")]
        [TestCase("Mozilla/5.0 (Linux; Android 6.0.1; SM-J700M Build/MMB29K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Mobile Safari/537.36", OSNames.Android, "6", "0", BrowserNames.Chrome, "69", "0")]
        [TestCase("Mozilla/5.0 (iPhone; CPU iPhone OS 12_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1", OSNames.IOS, "12", "1", BrowserNames.Safari, "12", "0")]
        [TestCase("Mozilla/5.0 (iPad; CPU OS 11_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/11.0 Mobile/15E148 Safari/604.1", OSNames.IOS, "11", "3", BrowserNames.Safari, "11", "0")]
        [TestCase("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0.1 Safari/605.1.15", OSNames.MacOSX, "10", "14", BrowserNames.Safari, "12", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36", OSNames.Windows, "10", "0", BrowserNames.Chrome, "74", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.117 Safari/537.36", OSNames.Windows, "7", "0", BrowserNames.Chrome, "33", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 6.3; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.113 Safari/537.36", OSNames.Windows, "6", "3", BrowserNames.Chrome, "60", "0")]
        //[TestCase("Mozilla/5.0 (Linux; U; Android 4.2.2; en-us; GT-I9100 Build/JDQ39E) AppleWebKit/534.30 (KHTML, like Gecko) Version/4.0 Mobile Safari/534.30 CyanogenMod/10.1.3/i9100", OSNames.Android, "4", "2", BrowserNames.AndroidBrowser, "4", "0")]
        //[TestCase("Mozilla/4.0 (compatible; MSIE 7.0b; Windows NT 6.0)", OSNames.Windows, "6", "0", BrowserNames.IE, "7", "0b")]
        //[TestCase(OSNames.Windows, "6", "1", "Opera/9.80 (Windows NT 6.1; WOW64; MRA 5.8 (build 4133)) Presto/2.12.388 Version/12.15")]
        //[TestCase(OSNames.Windows, "5", "1", "Mozilla/4.0 (compatible; MSIE 6.1; Windows XP; .NET CLR 1.1.4322; .NET CLR 2.0.50727)")]
        //[TestCase(OSNames.Windows, "4", "0", "Mozilla/4.0 (compatible; MSIE 4.5; Windows NT; )")]
        //[TestCase(OSNames.Windows, "", "", "Mozilla/4.0 (compatible; MSIE 2.0; Windows 95; Trident/4.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0)")]
        //[TestCase(OSNames.Android, "4", "1", "Opera/12.02 (Android 4.1; Linux; Opera Mobi/ADR-1111101157; U; en-US) Presto/2.9.201 Version/12.02")]
        //[TestCase(OSNames.Linux, "", "", "Mozilla/5.0 (X11; U; Linux armv7l; en-US) AppleWebKit/534.16 (KHTML, like Gecko) Chrome/10.0.648.204 Safari/534.16")]
        [TestCase("Mozilla/5.0 (iPad; CPU OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A5355d Safari/8536.25", OSNames.IOS, "6", "0", BrowserNames.Safari, "6", "0")]
        [TestCase("Mozilla/5.0 (iPad; CPU OS 5_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko ) Version/5.1 Mobile/9B176 Safari/7534.48.3", OSNames.IOS, "5", "1", BrowserNames.Safari, "5", "1")]
        [TestCase("Mozilla/5.0 (iPod; U; CPU iPhone OS 4_3_3 like Mac OS X; ja-jp) AppleWebKit/533.17.9 (KHTML, like Gecko) Version/5.0.2 Mobile/8J2 Safari/6533.18.5", OSNames.IOS, "4", "3", BrowserNames.Safari, "5", "0")]
        [TestCase("Mozilla/5.0 (iPhone Simulator; U; CPU iPhone OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.10 (KHTML, like Gecko) Version/4.0.4 Mobile/7D11 Safari/531.21.10", OSNames.IOS, "3", "2", BrowserNames.Safari, "4", "0")]

        //[TestCase(OSNames.MacOSX, "", "", "Mozilla/5.0 (Macintosh; U; PPC Mac OS X; it-it) AppleWebKit/124 (KHTML, like Gecko) Safari/125.1")]
        //[TestCase(OSNames.MacOSX, "", "", "Mozilla/5.0 (Macintosh; U; PPC Mac OS X; it-it) AppleWebKit/124 (KHTML, like Gecko) Safari/125.1")]
        //[TestCase(OSNames.MacOSX, "10", "6", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_6_8) AppleWebKit/537.13+ (KHTML, like Gecko) Version/5.1.7 Safari/534.57.2")]
        //[TestCase(OSNames.Macintosh, "", "", "Mozilla/4.0 (compatible; MSIE 5.21; Mac_PowerPC)")]
        //[TestCase(OSNames.Windows, "10", "0", "Mozilla/5.0 (Windows NT 10.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.71 Safari/537.36 Edge/12.0")]
        //[TestCase(OSNames.ChromeOS, "", "", "Mozilla/5.0 (X11; CrOS x86_64 6783.1.0) AppleWebKit/537.36 (KHTML, like Gecko) Edge/12.0")]
        //[TestCase(OSNames.Linux, "", "", "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/538.36 (KHTML, like Gecko) Edge/12.1")]
        [TestCase("Mozilla/5.0 (Windows NT 6.1; WOW64; rv:54.0) Gecko/20100101 Firefox/54.0", OSNames.Windows, "7", "0", BrowserNames.Firefox, "54", "0")]
        [TestCase("Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko", OSNames.Windows, "7", "0", BrowserNames.IE, "11", "0")]
        [TestCase("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)", OSNames.Windows, "7", "0", BrowserNames.IE, "10", "0")]
        [TestCase("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/74.0.3729.157 Safari/537.36", OSNames.Linux, "", "", BrowserNames.Chrome, "74", "0")]
        [TestCase("Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:50.0) Gecko/20100101 Firefox/50.0", OSNames.Linux, "", "", BrowserNames.Firefox, "50", "0")]
        [TestCase("Mozilla/5.0 (Linux; Android 6.0.1; SM-J700M Build/MMB29K) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Mobile Safari/537.36", OSNames.Android, "6", "0", BrowserNames.Chrome, "69", "0")]
        [TestCase("Mozilla/5.0 (iPhone; CPU iPhone OS 12_1 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1", OSNames.IOS, "12", "1", BrowserNames.Safari, "12", "0")]
        [TestCase("Mozilla/5.0 (iPad; CPU OS 11_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/11.0 Mobile/15E148 Safari/604.1", OSNames.IOS, "11", "3", BrowserNames.Safari, "11", "0")]
        [TestCase("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_1) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0.1 Safari/605.1.15", OSNames.MacOSX, "10", "14", BrowserNames.Safari, "12", "0")]
        public void TestUAParsing(string uaStr,
            string expectedOs,
            string expectedOsMajorVersion,
            string expectedOsMinorVersion,
            string expectedBrowser,
            string expectedBrowserMajorVersion,
            string expectedBrowserMinorVersion)
        {
            UserAgent ua = UserAgent.ParseUserAgentString(uaStr);
            Assert.AreEqual(expectedOs, ua.OS, "OS");
            Assert.AreEqual(expectedOsMajorVersion, ua.OSMajorVersion, "OS Major Version");
            Assert.AreEqual(expectedOsMinorVersion, ua.OSMinorVersion, "OS Minor Version");
            Assert.AreEqual(expectedBrowser, ua.Browser, "Browser");
            Assert.AreEqual(expectedBrowserMajorVersion, ua.BrowserMajorVersion, "Browser Major Version");
            Assert.AreEqual(expectedBrowserMinorVersion, ua.BrowserMinorVersion, "Browser Minor Version");
        }
    }
}
