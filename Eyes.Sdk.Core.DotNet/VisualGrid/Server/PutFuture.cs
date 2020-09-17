using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.VisualGrid
{
    public class PutFuture
    {
        private Task<WebResponse> putFuture_;
        private bool isSentAlready_;
        private RGridResource resource_;
        private readonly RunningRender runningRender_;
        private IEyesConnector eyesConnector_;
        private Logger logger_;

        private int retryCount_ = 5;

        public PutFuture(RGridResource resource, RunningRender runningRender, IEyesConnector eyesConnector, Logger logger)
        {
            resource_ = resource;
            runningRender_ = runningRender;
            eyesConnector_ = eyesConnector;
            logger_ = logger;
        }

        public PutFuture(Task<WebResponse> putFuture, RGridResource resource, RunningRender runningRender, IEyesConnector eyesConnector, Logger logger)
            : this(resource, runningRender, eyesConnector, logger)
        {
            putFuture_ = putFuture;
        }

        public bool Cancel(bool mayInterruptIfRunning)
        {
            return false;
        }

        public bool IsCancelled()
        {
            return false;
        }

        public bool IsDone()
        {
            return false;
        }

        public bool Get()
        {
            if (putFuture_ == null)
            {
                PutFuture newFuture = eyesConnector_.RenderPutResource(runningRender_, resource_);
                putFuture_ = newFuture.putFuture_;
            }
            if (!isSentAlready_)
            {
                while (retryCount_ != 0)
                {
                    try
                    {
                        WebResponse response = putFuture_.GetAwaiter().GetResult();
                        response?.Close();
                        break;
                    }
                    catch (Exception e)
                    {
                        logger_.Verbose("{0} on hash: {1}", e.Message, resource_.Sha256);
                        retryCount_--;
                        logger_.Verbose("Entering retry");
                        Thread.Sleep(300);
                        PutFuture newFuture = eyesConnector_.RenderPutResource(runningRender_, resource_);
                        logger_.Log("fired retry");
                        putFuture_ = newFuture.putFuture_;
                    }
                }
            }
            isSentAlready_ = true;
            return true;
        }

        public bool Get(TimeSpan timeout)
        {
            if (!isSentAlready_)
            {
                Task result = Task.WhenAny(putFuture_, Task.Delay(timeout)).Result;
                if (result != putFuture_)
                {
                    throw new Exception("timeout");
                }
                WebResponse response = ((Task<WebResponse>)result).Result;
                response?.Close();
            }
            isSentAlready_ = true;
            return true;
        }

        public override string ToString()
        {
            return $"{nameof(PutFuture)} - {resource_}";
        }
    }
}