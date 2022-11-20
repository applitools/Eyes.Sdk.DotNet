using System.Collections.Generic;
using System.Drawing;
using Applitools.Fluent;
using Applitools.Selenium.Commands;

namespace Applitools
{
    public class FloatingRegionByRectangle : IGetFloatingRegion
    {
        private readonly int _maxDownOffset;
        private readonly int _maxLeftOffset;
        private readonly int _maxRightOffset;
        private readonly int _maxUpOffset;

        private Rectangle rect_;

        public FloatingRegionByRectangle(Rectangle rect, int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
        {
            rect_ = rect;
            _maxUpOffset = maxUpOffset;
            _maxDownOffset = maxDownOffset;
            _maxLeftOffset = maxLeftOffset;
            _maxRightOffset = maxRightOffset;
        }

        public FloatingRegionByRectangle(Point location, Size size, int maxUpOffset = 0, int maxDownOffset = 0, int maxLeftOffset = 0, int maxRightOffset = 0)
        {
            rect_ = new Rectangle(location, size);
            _maxDownOffset = maxDownOffset;
            _maxUpOffset = maxUpOffset;
            _maxLeftOffset = maxLeftOffset;
            _maxRightOffset = maxRightOffset;
        }

        public IList<FloatingMatchSettings> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot)
        {
            return new[] {
                new FloatingMatchSettings() {
                    Left = rect_.Left,
                    Top = rect_.Top,
                    Width = rect_.Width,
                    Height = rect_.Height,
                    MaxLeftOffset = _maxLeftOffset,
                    MaxUpOffset = _maxUpOffset,
                    MaxRightOffset= _maxRightOffset,
                    MaxDownOffset = _maxDownOffset
                }
            };
        }

        public TFloatingRegion ToRegion()
        {
            return new RectangleFloatingRegion
            {
                Region = new UniversalRegion
                {
                    X = rect_.Left,
                    Y = rect_.Top,
                    Width = rect_.Width,
                    Height = rect_.Height
                },
                Offset = new Padding(_maxLeftOffset, _maxUpOffset, _maxRightOffset, _maxDownOffset)
            };
        }
    }
}