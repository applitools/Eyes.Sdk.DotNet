using Applitools.Utils;

namespace Applitools.Selenium.Common
{
    public static class Resources
    {
        internal static readonly string PROCESS_PAGE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePoll.js");
        internal static readonly string PROCESS_PAGE_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.processPagePollForIE.js");
        internal static readonly string POLL_PP_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResult.js");
        internal static readonly string POLL_PP_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_snapshot.dist.pollResultForIE.js");

        internal static readonly string CAPTURE_DOM = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_capture.dist.captureDomAndPoll.js");
        internal static readonly string CAPTURE_DOM_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_capture.dist.captureDomAndPollForIE.js");
        internal static readonly string POLL_CD_RESULT = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_capture.dist.pollResult.js");
        internal static readonly string POLL_CD_RESULT_FOR_IE = CommonUtils.ReadResourceFile("Eyes.Selenium.Common.Properties.NodeResources.node_modules._applitools.dom_capture.dist.pollResultForIE.js");
    }
}
