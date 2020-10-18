using System;
using System.Collections.Generic;

namespace Applitools.Selenium
{
    public class ClassicRunner : EyesRunner
    {
        private List<TestResults> allTestResult_ = new List<TestResults>();

        public EyesException Exception { get; internal set; }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            if (shouldThrowException && Exception != null)
            {
                throw new Exception("An error had occured", Exception);
            }
            List<TestResultContainer> result = new List<TestResultContainer>();
            foreach (TestResults testResults in allTestResult_)
            {
                result.Add(new TestResultContainer(testResults, null, null));
                EyesBase.LogSessionResultsAndThrowException(Logger, shouldThrowException, testResults);
            }

            return new TestResultsSummary(result);
        }

        internal void AggregateResult(TestResults testResult)
        {
            allTestResult_.Add(testResult);
        }
    }
}
