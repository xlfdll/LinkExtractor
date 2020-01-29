using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace LinkExtractor
{
    public class LinkExtractionWorker : IDisposable
    {
        public LinkExtractionWorker(HandlerDispatcher handlerDispatcher)
        {
            this.HandlerDispatcher = handlerDispatcher;

            this.SourceQueue = new ConcurrentQueue<String>();
            this.ResultQueue = new ConcurrentQueue<LinkItem>();

            this.CancellationTokenSource = new CancellationTokenSource();
            this.LinkExtractionTask = new Task(LinkExtractionTaskMethod, this.CancellationTokenSource.Token);
        }

        public HandlerDispatcher HandlerDispatcher { get; }

        public void Start()
        {

        }

        public void Stop()
        {
            this.CancellationTokenSource.Cancel();
        }

        public void EnqueueSource(String source)
        {
            this.SourceQueue.Enqueue(source);
        }

        private void LinkExtractionTaskMethod()
        {
            while (!this.CancellationTokenSource.Token.IsCancellationRequested)
            {
                String source = null;

                if (this.SourceQueue.TryDequeue(out source))
                {
                    
                }
            }
        }

        private ConcurrentQueue<String> SourceQueue { get; }
        private ConcurrentQueue<LinkItem> ResultQueue { get; }

        private Task LinkExtractionTask { get; }
        private CancellationTokenSource CancellationTokenSource { get; }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}