using System;
using Applitools.Commands;
using Applitools.Fluent;
using Applitools.Playwright.Universal.Dto;
using Applitools.Universal;
using Microsoft.Playwright;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright.Fluent
{
    public class FloatingRegionByElement : Element, IGetFloatingRegionOffsets, IGetFloatingRegion, IPlaywrightReference<TFloatingRegion>
    {
        public FloatingRegionByElement(IElementHandle element, 
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

        public FloatingRegionByElement(IElementHandle element, int maxOffset)
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
        
        TFloatingRegion IGetFloatingRegion.ToRegion()
        {
            var bbox = ElementHandle.BoundingBoxAsync().GetAwaiter().GetResult();
            return new RectangleFloatingRegion
            {
                Region = new UniversalRegion
                {
                    X = (int)Math.Round(bbox.X),
                    Y = (int)Math.Round(bbox.Y),
                    Width = (int)Math.Round(bbox.Width),
                    Height = (int)Math.Round(bbox.Height)
                },
                Offset = new Padding(MaxLeftOffset, MaxUpOffset, MaxRightOffset, MaxDownOffset)
            };
        }
        
        TFloatingRegion IPlaywrightReference<TFloatingRegion>.ToRegion(Reference root, Refer refer)
        {
            return new ElementFloatingRegion
            {
                Region = new RegionElement
                {
                    ElementId = refer.Ref(ElementHandle, root),
                },
                Offset = new Padding(MaxLeftOffset, MaxUpOffset, MaxRightOffset, MaxDownOffset)
            };
        }
    }
}