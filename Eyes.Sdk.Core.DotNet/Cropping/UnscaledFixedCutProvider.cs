using Applitools.Utils.Cropping;

namespace Applitools.Cropping
{
    public class UnscaledFixedCutProvider : FixedCutProvider, ICutProvider
    {
        public UnscaledFixedCutProvider(int header, int footer, int left, int right)
            : base(header, footer, left, right)
        {
        }

        public override ICutProvider Scale(double scaleRatio)
        {
            if (this is NullCutProvider)
            {
                return this;
            }
            return new UnscaledFixedCutProvider(header_, footer_, left_, right_);
        }

        public override string ToString()
        {
            return $"{nameof(UnscaledFixedCutProvider)} {HLFR}";
        }
    }
}
