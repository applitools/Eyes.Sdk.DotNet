using Applitools.VisualGrid;
using System.Collections.Generic;

namespace Applitools
{
    public interface IEyes : IEyesBase
    {
        void Check(ICheckSettings checkSettings);

        void Check(params ICheckSettings[] checkSettings);

        IDictionary<string, IRunningTest> GetAllRunningTests();

    }
}
