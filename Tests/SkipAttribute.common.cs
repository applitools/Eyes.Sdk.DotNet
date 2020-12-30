using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Applitools.Tests.Utils
{
    public class SkipAttribute : NUnitAttribute, IApplyToTest
    {
        private string reason_;
        public SkipAttribute(string reason)
        {
            reason_ = reason;
        }

        public void ApplyToTest(Test test)
        {
            if (test.RunState != RunState.NotRunnable)
            {
                test.RunState = RunState.Skipped;
                test.Properties.Set("_SKIPREASON", reason_);
            }
        }
    }
}
