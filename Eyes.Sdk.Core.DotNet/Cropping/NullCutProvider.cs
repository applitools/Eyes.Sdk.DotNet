using Applitools.Utils.Cropping;

namespace Applitools.Cropping
{
    /// <summary>
    /// A cut provider that does noting
    /// </summary>
    public class NullCutProvider : UnscaledFixedCutProvider, ICutProvider
    {
        /// <summary>
        /// The only constructor.
        /// </summary>
        public NullCutProvider() : base(0, 0, 0, 0) { }

        /// <summary>
        /// Static accessor to an instance.
        /// </summary>
        public static readonly NullCutProvider Instance = new NullCutProvider();

        public override string ToString()
        {
            return nameof(NullCutProvider);
        }
    }
}