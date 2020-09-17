namespace Applitools
{
    using Utils;

    /// <summary>
    /// A scale provider based on a fixed scale ratio.
    /// </summary>
    public class FixedScaleProvider : IScaleProvider
    {
        
        /// <summary>
        /// creates a new <see cref="FixedScaleProvider" /> instance.
        /// </summary>
        /// <param name="scaleRatio">The scale ratio to use.</param>
        public FixedScaleProvider(double scaleRatio)
        {
            ArgumentGuard.GreaterThan(scaleRatio, 0, nameof(scaleRatio));

            ScaleRatio = scaleRatio;
        }

        public double ScaleRatio { get; private set; }

        public override string ToString()
        {
            return $"{nameof(FixedScaleProvider)} (ScaleRatio: {ScaleRatio})";
        }
    }
}
