using System.Collections.Generic;

namespace Applitools
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class ImageMatchOptions
    {
        public ImageMatchOptions(ImageMatchSettings imageMatchSettings)
        {
            ImageMatchSettings = imageMatchSettings;
        }

        public string Name { get; set; }
        public string Source { get; set; }
        public string RenderId { get; set; }

        public IList<Trigger> UserInputs { get; set; }

        public ImageMatchSettings ImageMatchSettings { get; set; }

        public bool ForceMatch { get; set; }
        public bool ForceMismatch { get; set; }
        public bool IgnoreMatch { get; set; }
        public bool IgnoreMismatch { get; set; }
        public bool ReplaceLast { get; set; }
        public string VariantId { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
