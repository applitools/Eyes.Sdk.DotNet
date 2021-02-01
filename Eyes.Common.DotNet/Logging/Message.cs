using System;
using System.Collections.Generic;

namespace Applitools
{
    public class Message
    {
        public string AgentId { get; }

        public Stage Stage { get; }

        public Type Type { get; }

        public long ThreadId { get; }

        public string StackTrace { get; }

        public HashSet<string> TestId { get; }

        public Dictionary<string, object> Data { get; }

        public Message() { }

        public Message(string agentId, Stage stage, Type type, HashSet<string> testId, long threadId,
                       string stackTrace, Dictionary<string, object> data)
        {
            AgentId = agentId;
            Stage = stage;
            Type = type;
            TestId = testId ?? new HashSet<string>();
            ThreadId = threadId;
            StackTrace = stackTrace;
            Data = data;
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o == null || GetType() != o.GetType()) return false;
            Message message = (Message)o;
            return ThreadId == message.ThreadId &&
                Equals(AgentId, message.AgentId) &&
                Stage == message.Stage &&
                Type == message.Type &&
                Equals(TestId, message.TestId) &&
                Equals(StackTrace, message.StackTrace) &&
                Equals(Data, message.Data);
        }

        public override int GetHashCode()
        {
            object[] arr = new object[] { AgentId, Stage, Type, TestId, ThreadId, StackTrace, Data };
            return GetHashCode(arr);
        }

        public static int GetHashCode(object[] a)
        {
            if (a == null)
                return 0;

            int result = 1;

            foreach (object element in a)
                result = 31 * result + (element == null ? 0 : element.GetHashCode());

            return result;
        }
    }
}
