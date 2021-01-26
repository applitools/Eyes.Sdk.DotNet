using System;
using System.Collections.Generic;

namespace Applitools
{
    public abstract class EyesService<TInput, TOutput>
    {

        protected readonly Queue<Tuple<string, TInput>> inputQueue_ = new Queue<Tuple<string, TInput>>();
        protected readonly List<Tuple<string, TOutput>> outputQueue_ = new List<Tuple<string, TOutput>>();
        protected readonly List<Tuple<string, Exception>> errorQueue_ = new List<Tuple<string, Exception>>();

        protected Logger Logger { get; private set; }
        protected internal IServerConnector ServerConnector { get; set; }

        public EyesService(Logger logger, IServerConnector serverConnector)
        {
            Logger = logger;
            ServerConnector = serverConnector;
        }

        public abstract void Run();

        public void AddInput(string id, TInput input)
        {
            inputQueue_.Enqueue(new Tuple<string, TInput>(id, input));
        }

        public IList<Tuple<string, TOutput>> GetSucceededTasks()
        {
            lock (outputQueue_)
            {
                Tuple<string, TOutput>[] array = outputQueue_.ToArray();
                outputQueue_.Clear();
                return array;
            }
        }

        public IList<Tuple<string, Exception>> GetFailedTasks()
        {
            lock (errorQueue_)
            {
                Tuple<string, Exception>[] array = errorQueue_.ToArray();
                errorQueue_.Clear();
                return array;
            }
        }
    }
}