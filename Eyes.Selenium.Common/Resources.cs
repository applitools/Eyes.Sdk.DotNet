using Applitools.Utils;
using System;

namespace Applitools.Selenium.Common
{
    public class Resources
    {
        internal static readonly string PROCESS_PAGE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePoll.js");
        internal static readonly string PROCESS_PAGE_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePollForIE.js");
        internal static readonly string POLL_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResult.js");
        internal static readonly string POLL_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResultForIE.js");
    }
}
