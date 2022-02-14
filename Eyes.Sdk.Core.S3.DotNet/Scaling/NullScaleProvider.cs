namespace Applitools
{
    public class NullScaleProvider : FixedScaleProvider
    {
        public NullScaleProvider() : base(1.0) { }

        public static readonly NullScaleProvider Instance = new NullScaleProvider();

        public override string ToString()
        {
            return nameof(NullScaleProvider);
        }
    }
}
