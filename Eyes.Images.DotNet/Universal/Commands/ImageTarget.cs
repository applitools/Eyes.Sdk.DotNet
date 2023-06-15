using Applitools.Utils.Geometry;

namespace Applitools.Commands
{
    public class ImageTarget : ITarget
    {
        public string Image { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Dom { get; set; }
        public Location LocationInViewport { get; set; }
        public Location LocationInView { get; set; }
        public RectangleSize FullViewSize { get; set; }
    }
}
