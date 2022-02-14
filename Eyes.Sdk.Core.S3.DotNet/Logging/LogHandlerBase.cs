using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Applitools
{
    public abstract class LogHandlerBase : ILogHandler
    {
        protected bool isOpen_;
        private readonly static object lock_ = new object();

        protected LogHandlerBase(bool isVerbose)
        {
            MinimumTraceLevel = isVerbose ? TraceLevel.Debug : TraceLevel.Notice;
        }

        protected LogHandlerBase(TraceLevel minimumTraceLevel)
        {
            MinimumTraceLevel = minimumTraceLevel;
        }

        public TraceLevel MinimumTraceLevel { get; }

        public virtual void Open() { }

        public virtual void OnMessage(TraceLevel level, string message, params object[] args)
        {
            if (level >= MinimumTraceLevel)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                OnMessage(message, level);
            }
        }

        public virtual void OnMessage(TraceLevel level, Func<string> messageProvider)
        {
            if (level >= MinimumTraceLevel)
            {
                OnMessage(messageProvider(), level);
            }
        }

        public abstract void OnMessage(string message, TraceLevel level = default);


        public virtual void OnMessage(ClientEvent @event)
        {
            if (@event.Level >= MinimumTraceLevel)
            {
                lock (lock_)
                {
                    OnMessage(JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Converters = new[] { new StringEnumConverter() }
                    }), @event.Level);
                }
            }
        }

        public virtual void Close() { }

        public bool IsOpen => isOpen_;
    }

}
