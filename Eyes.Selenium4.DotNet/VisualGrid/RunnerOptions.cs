namespace Applitools.VisualGrid
{
    public class RunnerOptions : IRunnerOptionsInternal
    {
        public RunnerOptions() { }
        internal RunnerOptions(int concurrency)
        {
            concurrency_ = concurrency;
        }

        RunnerOptions(RunnerOptions other)
        {
            concurrency_ = other.concurrency_;
        }

        int concurrency_;

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
