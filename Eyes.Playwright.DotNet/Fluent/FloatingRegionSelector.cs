using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Microsoft.Playwright;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright.Fluent
{
    /// <summary>
    /// Used internally to represent a ref of an accessibility element
    /// </summary>
    public class FloatingRegionSelector : Selector, IGetFloatingRegionOffsets, IGetFloatingRegion,
        IPlaywrightReference<TFloatingRegion>
    {
        public int MaxUpOffset { get; }
        public int MaxDownOffset { get; }
        public int MaxLeftOffset { get; }
        public int MaxRightOffset { get; }

        public FloatingRegionSelector(string selector,
            int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
            : base(selector)
        {
            MaxUpOffset = maxUpOffset;
            MaxDownOffset = maxDownOffset;
            MaxLeftOffset = maxLeftOffset;
            MaxRightOffset = maxRightOffset;
        }

        public FloatingRegionSelector(string selector, int maxOffset)
            : base(selector)
        {
            MaxUpOffset = maxOffset;
            MaxDownOffset = maxOffset;
            MaxLeftOffset = maxOffset;
            MaxRightOffset = maxOffset;
        }

        public FloatingRegionSelector(ILocator locator,
            int maxUpOffset, int maxDownOffset, int maxLeftOffset, int maxRightOffset)
            : base(locator)
        {
            MaxUpOffset = maxUpOffset;
            MaxDownOffset = maxDownOffset;
            MaxLeftOffset = maxLeftOffset;
            MaxRightOffset = maxRightOffset;
        }

        public FloatingRegionSelector(ILocator locator, int maxOffset)
            : base(locator)
        {
            MaxUpOffset = maxOffset;
            MaxDownOffset = maxOffset;
            MaxLeftOffset = maxOffset;
            MaxRightOffset = maxOffset;
        }

        TFloatingRegion IGetFloatingRegion.ToRegion()
        {
            throw new System.NotImplementedException();
        }

        TFloatingRegion IPlaywrightReference<TFloatingRegion>.ToRegion(Reference root, Refer refer)
        {
            return new SelectorFloatingRegion
            {
                Region = GetRegionSelector(root, refer),
                Offset = new Padding(MaxLeftOffset, MaxUpOffset, MaxRightOffset, MaxDownOffset)
            };
        }
    }
}