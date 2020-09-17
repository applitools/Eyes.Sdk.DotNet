using Applitools.Utils;
using System.Collections;
using System.Collections.Generic;

namespace Applitools
{
    public class TestResultsSummary : IReadOnlyCollection<TestResultContainer>
    {
        private List<TestResultContainer> allResults_;
        private readonly int passed_ = 0;
        private readonly int unresolved_ = 0;
        private readonly int failed_ = 0;
        private readonly int exceptions_ = 0;
        private readonly int mismatches_ = 0;
        private readonly int missing_ = 0;
        private readonly int matches_ = 0;

        public TestResultsSummary(List<TestResultContainer> allResults)
        {
            allResults_ = allResults;
            foreach (TestResultContainer resultContainer in allResults)
            {
                if (resultContainer == null) continue;
                if (resultContainer.Exception != null)
                {
                    exceptions_++;
                }
                TestResults result = resultContainer.TestResults;
                if (result == null) continue;
                switch (result.Status)
                {
                    case TestResultsStatus.Failed: failed_++; break;
                    case TestResultsStatus.Passed: passed_++; break;
                    case TestResultsStatus.Unresolved: unresolved_++; break;
                }
                matches_ += result.Matches;
                missing_ += result.Missing;
                mismatches_ += result.Mismatches;
            }
        }
        public TestResultContainer[] GetAllResults()
        {
            return allResults_.ToArray();
        }

        public TestResultContainer this[int index] => allResults_[index];

        public override string ToString()
        {
            return "result summary {" +
                    "\n\tall results=\n\t\t" + StringUtils.Concat(allResults_, "\n\t\t") +
                    "\n\tpassed=" + passed_ +
                    "\n\tunresolved=" + unresolved_ +
                    "\n\tfailed=" + failed_ +
                    "\n\texceptions=" + exceptions_ +
                    "\n\tmismatches=" + mismatches_ +
                    "\n\tmissing=" + missing_ +
                    "\n\tmatches=" + matches_ +
                    "\n}";
        }

        public int Count => ((IReadOnlyCollection<TestResultContainer>)allResults_).Count;

        public IEnumerator<TestResultContainer> GetEnumerator()
        {
            return ((IReadOnlyCollection<TestResultContainer>)allResults_).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IReadOnlyCollection<TestResultContainer>)allResults_).GetEnumerator();
        }
    }
}
