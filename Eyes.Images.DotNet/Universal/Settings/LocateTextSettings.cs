using System.Collections.Generic;

namespace Applitools
{
    public class LocateTextSettings : SettingsBase
    {
        public ICollection<object> Patterns { get; set; }
        public bool? IgnoreCase { get; set; }
        public bool? FirstOnly { get; set; }
        public string Language { get; set; }

        public LocateTextSettings()
        {
            Patterns = new List<object>();
        }
    }
}