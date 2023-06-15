using Applitools.Fluent;

namespace Applitools
{
    public class ExtractTextSettings
    {
        public TRegion Region { get; set; }
        public string Hint { get; set; }
        public int? MinMatch { get; set; }
        public string Language { get; set; }
    }
}