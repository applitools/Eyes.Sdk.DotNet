using Applitools.Playwright.Universal.Dto;
using Microsoft.Playwright;

namespace Applitools.Playwright.Fluent
{
    public class FloatingRegionElement : Element, IGetFloatingRegionOffsets
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
    }
}