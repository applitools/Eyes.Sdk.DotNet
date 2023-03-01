using System.Collections.Generic;
using System.Linq;

namespace Applitools
{
    public class TextRegionSettings
    {
        public ICollection<string> Patterns { get; private set; }
        public string Image { get; private set; }
        public bool? IgnoreCase { get; private set; }
        public bool? FirstOnly { get; private set; }
        public string Language { get; private set; }

        public TextRegionSettings(string pattern) 
            : this(new List<string>{pattern})
        {
            
        }

        public TextRegionSettings(params string[] patterns)
        : this(patterns.ToList())
        {
            
        }

        public TextRegionSettings(ICollection<string> patterns)
        {
            Patterns = patterns;
        }

        public TextRegionSettings SetIgnoreCase(bool? ignoreCase)
        {
            IgnoreCase = ignoreCase;
            return this;
        }

        public TextRegionSettings SetFirstOnly(bool? firstOnly)
        {
            FirstOnly = firstOnly;
            return this;
        }

        public TextRegionSettings SetLanguage(string language)
        {
            Language = language;
            return this;
        }

        public TextRegionSettings SetImage(string image)
        {
            Image = image;
            return this;
        }
    }
}