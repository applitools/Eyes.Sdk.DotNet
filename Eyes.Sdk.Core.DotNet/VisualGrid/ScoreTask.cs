namespace Applitools.VisualGrid
{
    public class ScoreTask
    {
        private readonly VisualGridTask task_;

        public ScoreTask(VisualGridTask task, int score)
        {
            task_ = task;
            Score = score;
        }

        public VisualGridTask Task
        {
            get
            {
                task_.IsSent = true;
                return task_;
            }
        }

        public int Score { get; }
    }
}