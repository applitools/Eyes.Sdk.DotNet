using Applitools.Commands;
using Applitools.Playwright.Universal.Dto;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public class FloatingRegionElement : Element, IGetFloatingRegionOffsets, IGetFloatingRegion
    {
        public FloatingRegionElement(IElementHandle element, 
            int maxUpOffset, 
            int maxDownOffset, 
            int maxLeftOffset,
            int maxRightOffset)
        {
            ElementHandle = element;
            MaxUpOffset = maxUpOffset;
            MaxDownOffset = maxDownOffset;
            MaxLeftOffset = maxLeftOffset;
            MaxRightOffset = maxRightOffset;
        }

        public FloatingRegionElement(IElementHandle element, int maxOffset)
        {
            ElementHandle = element;
            MaxUpOffset = maxOffset;
            MaxDownOffset = maxOffset;
            MaxLeftOffset = maxOffset;
            MaxRightOffset = maxOffset;
        }

        public int MaxLeftOffset { get; }
        public int MaxUpOffset { get; }
        public int MaxRightOffset { get; }
        public int MaxDownOffset { get; }
        
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