using System.Drawing;

namespace Applitools.Selenium.Common
{
    internal interface IEyesWebDriverCommon
    {
        object GetSessionDetail(string detail);
        int GetFrameChainLength();
        Size GetDefaultContentViewportSize(bool forceQuery = false);
    }
}
