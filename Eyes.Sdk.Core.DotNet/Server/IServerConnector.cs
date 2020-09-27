using Applitools.Ufg;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools
{
    public interface IServerConnector : IUfgConnector, IDeleteSession
    {
        TimeSpan Timeout { get; set; }
        string SdkName { get; set; }
        bool DontCloseBatches { get; }

        void CloseBatch(string batchId);
        MatchResult MatchWindow(RunningSession runningSession, MatchWindowData data);
        TestResults EndSession(RunningSession runningSession, bool isAborted, bool save);
        string[] GetTextInRunningSessionImage(RunningSession runningSession, string imageId, IList<Rectangle> regions, string language);
        string AddRunningSessionImage(RunningSession runningSession, byte[] imageBytes);
        string PostDomCapture(string domJson);
        RunningSession StartSession(SessionStartInfo sessionStartInfo);
    }
}
