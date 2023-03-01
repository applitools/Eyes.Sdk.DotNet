using System.Collections.Generic;

namespace Applitools
{
    public class ExtractTextRegionsSettings
    {
        public ICollection<string> Patterns { get; set; }
        public int? IgnoreCase { get; set; }
        public string Language { get; set; }
    }
}