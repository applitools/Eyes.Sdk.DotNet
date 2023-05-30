using System;
using System.Collections.Concurrent;
using Applitools.Commands;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools.Universal
{
    static class LockerClass
    {
        public static readonly object Locker = new object();
    }
    public static class Singleton<T>
    {
        private static T instance_;

        public static T GetInstance(Logger logger)
        {
            if (instance_ == null)
            {
                lock (LockerClass.Locker)
                {
                    if (instance_ == null)
                    {
                        instance_ = (T)Activator.CreateInstance(typeof(T), logger);
                    }
                }
            }

            return instance_;
        }
    }
    public class SpecDriverMessageListener : ISpecDriverMessageListener
    {
        private ConcurrentDictionary<string, WsMessage> messages_;
        protected Logger Logger { get; }
        protected static JsonSerializer Serializer { get; } = JsonUtils.CreateSerializer();

        public SpecDriverMessageListener()
        {
            Logger = new Logger();
        }
        
        public SpecDriverMessageListener(Logger logger)
        {
            Logger = logger;
        }

        public virtual void HandleMessage(string message)
        {
            HandleResponse_(message);
        }

        public WebSocketClient WebSocket { get; set; }

        public void SetMessagesDictionary(ConcurrentDictionary<string, WsMessage> messages)
        {
            messages_ = messages;
        }

        protected void HandleResponse_(string message)
        {
            try
            {
                var response = Serializer.Deserialize<CommandResponse>(message);
                if (messages_.TryGetValue(response.Key, out WsMessage wsMessage))
                {
                    wsMessage.IsHandled = true;
                }
            }
            catch (Exception e)
            {
                CommonUtils.LogExceptionStackTrace(Logger, Stage.SpecDriver, e);
            }
        }
    }
}