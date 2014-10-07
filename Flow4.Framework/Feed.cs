using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Framework
{
    public class Feed<T> : BaseController
    {
        object _lockObject = new object();
        internal T[] _queue;
        HashSet<FeedOutputQueue<T>> _outputQueues;

        public string Name { get; private set; }
        public int MaxQueueSize { get; private set; }
        internal int WritePosition { get; private set; }
        
        public Feed(string name, int maxQueueSize = 100)
            : base(1000)
        {
            this.Name = name;
            this.MaxQueueSize = maxQueueSize;

            this.WritePosition = -1;
            
            _queue = new T[MaxQueueSize];
            _outputQueues = new HashSet<FeedOutputQueue<T>>();
        }

        public IEnumerable<T> Subscribe()
        {
            var outputQueue = new FeedOutputQueue<T>(this);
            _outputQueues.Add(outputQueue);
            return outputQueue;
        }

        public void Enqueue(T item)
        {
            lock(_lockObject)
            {
                var nextPosition = WritePosition;

                if (nextPosition == -1)
                    nextPosition++;

                _queue[nextPosition++] = item;
                if (nextPosition == MaxQueueSize)
                    nextPosition = 0;
                
                WritePosition = nextPosition;

                verifyQueueNotOverflowed();
            }
        }

        protected override void OnHeartbeat(object state)
        {
            base.OnHeartbeat(state);
            evictEmptyQueueEntries();
        }

        private void evictEmptyQueueEntries()
        {
            if (WritePosition == -1)
                return;

            lock (_lockObject)
            {
                //var timer = new Stopwatch();
                //timer.Start();

                var readPositions = _outputQueues.Select(q => q.ReadPosition);

                //Clear array after write cursor to last read cursor or end or array
                var lastIndex = readPositions
                    .Where(p => p > WritePosition)
                    .DefaultIfEmpty(MaxQueueSize)
                    .Min();
                for (var i = WritePosition; i < lastIndex; i++)
                    _queue[i] = default(T);

                //If cleared to end, clear from beginning to last read cursor too
                if (lastIndex == MaxQueueSize)
                {
                    lastIndex = readPositions
                        .Where(p => p < WritePosition)
                        .DefaultIfEmpty(0)
                        .Min();
                    for (var i = 0; i < lastIndex; i++)
                        _queue[i] = default(T);
                }

                //timer.Stop();
                //Trace.TraceInformation("Evicting old feed entries took {0} ms.", timer.ElapsedMilliseconds);
            }
        }

        private void verifyQueueNotOverflowed()
        {
            if (_outputQueues.Any(p => p.ReadPosition == WritePosition))
            {
                throw new StackOverflowException(string.Format("Feed '{0}' overflowed.", Name));
            }
        }
    }


    public class FeedOutputQueue<T> : IEnumerable<T>, IEnumerator<T>
    {
        Feed<T> _feed;
        internal int ReadPosition { get; private set; }

        internal FeedOutputQueue(Feed<T> feed)
        {
            this._feed = feed;
            this.ReadPosition = -1;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            if (_feed.WritePosition == -1 || this.ReadPosition + 1 == _feed.WritePosition)
                return false;
            
            this.ReadPosition++;
            if (this.ReadPosition == _feed.MaxQueueSize)
                this.ReadPosition = 0;

            return true;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this;
        }

        public T Current
        {
            get {
                if (this.ReadPosition == -1)
                    throw new System.IO.EndOfStreamException("Beginning of stream. MoveNext first!");
                if (this.ReadPosition == _feed.WritePosition)
                    throw new System.IO.EndOfStreamException();
                return _feed._queue[this.ReadPosition];
            }
        }

        public void Dispose()
        {
            //Do nothing. This occurs each time the foreach() statement has emptied the enumerator
        }

        object System.Collections.IEnumerator.Current { get { return this.Current; } }

        public void Reset()
        {
            this.ReadPosition = _feed.WritePosition;
        }
    }
}
