namespace Applitools
{
    public class SessionStopInfo
    {
        public SessionStopInfo(RunningSession runningSession, bool isAborted, bool shouldSave)
        {
            RunningSession = runningSession;
            IsAborted = isAborted;
            ShouldSave = shouldSave;
        }

        public RunningSession RunningSession { get; set; }
        public bool IsAborted { get; set; }
        public bool ShouldSave { get; set; }
    }
}
