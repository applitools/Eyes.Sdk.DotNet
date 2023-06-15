using Applitools.Utils.Geometry;
using System.Drawing;

namespace Applitools.Metadata
{

    public partial class ImageIdentifier
    {
        public string Id { get; set; }

        public string DomId { get; set; }

        public RectangleSize Size { get; set; }

        public RectangleSize Viewport { get; set; }

        public Rectangle Rectangle { get; set; }

        public Location Location { get; set; }

        public bool HasDom { get; set; }
    }
}