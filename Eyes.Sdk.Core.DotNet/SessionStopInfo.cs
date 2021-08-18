using Newtonsoft.Json;

namespace Applitools
{
    public class SessionStopInfo
    {
        public SessionStopInfo(RunningSession runningSession, bool isAborted, bool shouldSave)
        {
            RunningSession = runningSession;
            Aborted = isAborted;
            UpdateBaseline = shouldSave;
        }

        [JsonIgnore]
        public RunningSession RunningSession { get; set; }
        public bool Aborted { get; set; }
        public bool UpdateBaseline { get; set; }

        public override string ToString()
        {
            return $"Session: {RunningSession} ; Aborted: {Aborted} ; Save: {UpdateBaseline}";
        }
    }
}