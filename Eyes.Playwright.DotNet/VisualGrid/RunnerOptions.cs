namespace Applitools.VisualGrid
{
    public class RunnerOptions : IRunnerOptionsInternal
    {
        private int concurrency_;

        public RunnerOptions()
        {
        }

        internal RunnerOptions(int concurrency)
        {
            concurrency_ = concurrency;
        }

        private RunnerOptions(RunnerOptions other)
        {
            concurrency_ = other.concurrency_;
        }

        public RunnerOptions TestConcurrency(int concurrency)
        {
            return new RunnerOptions(this) { concurrency_ = concurrency };
        }

        int IRunnerOptionsInternal.GetConcurrency()
        {
            return concurrency_;
        }
    }
}