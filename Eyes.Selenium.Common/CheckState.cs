using Applitools.Utils;
using System.Drawing;

namespace Applitools.Selenium
{
    internal class CheckState<IWebElement>
    {
        public IWebElement TargetElementInternal { get; set; }
        public bool StitchContent { get; set; }
        public IPositionProvider StitchPositionProvider { get; set; }
        public Rectangle EffectiveViewport { get; set; }
        public Rectangle FullRegion { get; set; }
        public IPositionProvider OriginPositionProvider { get; internal set; } = new NullPositionProvider();
        public Size StitchOffset { get; internal set; }
        public IWebElement FrameToSwitchTo { get; set; }
        public Point OriginLocation { get; set; }
    }
}