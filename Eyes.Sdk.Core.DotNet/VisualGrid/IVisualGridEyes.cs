using System.Collections.Generic;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public interface IVisualGridEyes
    {
        bool IsEyesClosed();

        RunningTest GetNextTestToClose();

        void SetListener(EyesListener listener);

        ScoreTask GetBestScoreTaskForCheck();

        ScoreTask GetBestScoreTaskForOpen();

        BatchInfo Batch { set; get; }

        Logger Logger { get; }

        ICollection<Task<TestResultContainer>> Close();

        ICollection<RunningTest> GetAllRunningTests();

        string PrintAllFutures();

        ICollection<TestResultContainer> TestResults { get; }

        IBatchCloser GetBatchCloser();
    }
}