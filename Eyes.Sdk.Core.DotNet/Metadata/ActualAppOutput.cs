using System;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class ActualAppOutput
    {
        public ImageIdentifier Image { get; set; }

        public ImageIdentifier Thumbprint { get; set; }

        public ImageMatchSettings ImageMatchSettings { get; set; }

        public bool IgnoreExpectedOutputSettings { get; set; }

        public bool IsMatching { get; set; }

        public bool AreImagesMatching { get; set; }

        public DateTime OccurredAt { get; set; }

        public object[] UserInputs { get; set; }

        public string WindowTitle { get; set; }

        public string Tag { get; set; }

        public bool IsPrimary { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}