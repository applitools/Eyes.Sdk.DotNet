using System.Collections.Generic;

namespace Applitools
{
    public interface IEyes : IEyesBase
    {
        void Check(ICheckSettings checkSettings);

        void Check(params ICheckSettings[] checkSettings);

        IDictionary<string, RunningTest> GetAllRunningTests();
        bool IsCompleted();
        IList<TestResultContainer> GetAllTestResults();
    }
}
