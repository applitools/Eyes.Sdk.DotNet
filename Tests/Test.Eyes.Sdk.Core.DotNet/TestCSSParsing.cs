using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Ufg.Model;
using Applitools.Utils;
using NUnit.Framework;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Applitools.Tests
{
    public class TestCSSParsing : ReportingTestSuite
    {
        [Test]
        public void TestBadCss()
        {
            string badCss = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.clientlibs_all.default.css");
            DomAnalyzer.TextualDataResource data = new DomAnalyzer.TextualDataResource()
            {
                Data = badCss,
                Uri = new System.Uri("https://a.co/path/")
            };
            Dictionary<string, FrameData> extraResources = new Dictionary<string, FrameData>();
            HashSet<string> expectedResources = new HashSet<string>(new string[]{
                "https://a.co/content/dam/everything-everywhere/5g/bottom-gradient-desktop.png",
                "https://a.co/content/dam/everything-everywhere/5g/desktop_top_gradient.png",
                "https://a.co/content/dam/everything-everywhere/5g/mobile_top_gradient.png",
                "https://a.co/content/dam/everything-everywhere/5g/mobile-bottom-gradient.png",
                "https://a.co/ee-common-2015/clientlibs_base/img/EE-close.png",
                "https://a.co/ee-common-2015/clientlibs_base/img/EE-main-sprite.svg",
                "https://a.co/ee-common-2015/clientlibs_base/img/EE-shop-sprite.png",
                "https://a.co/ee-common-2015/clientlibs_base/img/loading.gif",
                "https://a.co/ee-common-2015/clientlibs_base/img/twitter-icon-sprite.png",
                "https://a.co/ee-map-2019/clientlibs_storefinder/img/store-finder/coverage/ajax-loader.gif",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/ee-icons.ttf",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/ee-icons.woff",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/nobblee_light.ttf",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/nobblee_light.woff",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/nobblee_regular.ttf",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/nobblee_regular.woff",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/rubrik_regular.ttf",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/rubrik_regular.woff",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/rubrik_semibold.ttf",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/fonts/rubrik_semibold.woff",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/EE-main-sprite.png",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/EE-main-sprite.svg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/EE-shop-sprite.png",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_AQ_1024.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_AQ_1360.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_AQ_768.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_GY_1024.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_GY_1360.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_GY_768.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_IV_1024.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_IV_1360.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_IV_768.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_YE_1024.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_YE_1360.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/scoops/SCOOP_YE_768.jpg",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/smart-layer/SMART_LAYER_1024.png",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/smart-layer/SMART_LAYER_1360.png",
                "https://a.co/etc/designs/ee-common-2015/clientlibs_base/img/smart-layer/SMART_LAYER_768.png",
            });
            Logger logger = new Logger();
            ILogHandler logHandler = TestUtils.InitLogHandler();
            logger.SetLogHandler(logHandler);

            ConcurrentDictionary<string, HashSet<string>> cache = new ConcurrentDictionary<string, HashSet<string>>();
            DomAnalyzer.ParseCSS_(data, extraResources, logger, cache);
            logger.Log("expected:");
            foreach (string url in expectedResources)
            {
                logger.Log(url);
            }
            logger.Log("actual:");
            foreach (string url in extraResources.Keys)
            {
                logger.Log(url);
            }

            Thread.Sleep(2000);
            logHandler.Close();

            CollectionAssert.AreEquivalent(expectedResources, extraResources.Keys);
        }
    }
}
