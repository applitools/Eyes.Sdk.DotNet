using System.Drawing;

namespace Applitools.Utils.Geometry
{
    public struct SubregionForStitching
    {
        public SubregionForStitching(Point scrollTo, Point pastePhysicalLocation, Rectangle physicalCropArea, Rectangle logicalCropArea) : this()
        {
            ScrollTo = scrollTo;
            PasteLocation = pastePhysicalLocation;
            PhysicalCropArea = physicalCropArea;
            LogicalCropArea = logicalCropArea;
        }

        public Point ScrollTo { get; }
        public Point PasteLocation { get; }
        public Rectangle PhysicalCropArea { get; }
        public Rectangle LogicalCropArea { get; }

        public override string ToString()
        {
            return $"ScrollTo: {ScrollTo} ; PasteLocation: {PasteLocation} ; PhysicalCropArea: {PhysicalCropArea} ; LogicalCropArea {LogicalCropArea}";
        }
    }
}