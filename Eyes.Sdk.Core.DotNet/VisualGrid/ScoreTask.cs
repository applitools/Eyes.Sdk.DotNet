namespace Applitools.VisualGrid
{
    public class ScoreTask
    {
        public ScoreTask(VisualGridTask task, int score)
        {
            Task = task;
            Score = score;
        }

        public VisualGridTask Task { get; }

        public int Score { get; }
    }
}