using Flow4.Entities.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Framework
{
    public class Feed<T> : BaseController, IFeed<T>
        where T : IRefCountedEntity
    {
        object _lockObject = new object();
        internal T[] _queue;
        HashSet<FeedOutputQueue<T>> _outputQueues;
        bool isDisposed = false;

        public string Name { get; private set; }
        public int MaxQueueSize { get; private set; }
        internal int WritePosition { get; private set; }
        
        public Feed(string name, int capacity = 10)
            : base(1000)
        {
            this.Name = name;
            this.MaxQueueSize = capacity;

            this.WritePosition = -1;
            
            _queue = new T[MaxQueueSize];
            _outputQueues = new HashSet<FeedOutputQueue<T>>();
        }

        protected override void Dispose(bool disposing)
        {
            lock(_lockObject)
            {
                this.Stop();
                isDisposed = true;
                if (_outputQueues.Any())
                    throw new ApplicationException("Feed with subscribers is being disposed!");
                disposeQueue(0, MaxQueueSize);
                _queue = null;
            }
        }

        public FeedOutputQueue<T> Subscribe()
        {
            lock (_lockObject)
            {
                if (isDisposed)
                    throw new ApplicationException("Feed is already disposed");
                var outputQueue = new FeedOutputQueue<T>(this);
                _outputQueues.Add(outputQueue);
                return outputQueue;
            }
        }

        internal void Unsubscribe(FeedOutputQueue<T> outputQueue)
        {
            _outputQueues.Remove(outputQueue);
        }

        public void Enqueue(T item)
        {
            lock(_lockObject)
            {
                if (isDisposed)
                    throw new ApplicationException("Trying to add to a disposed queue.");

                var nextPosition = WritePosition;

                if (nextPosition == -1)
                    nextPosition++;

                _queue[nextPosition++] = item;

                if (nextPosition == MaxQueueSize)
                    nextPosition = 0;
                
                WritePosition = nextPosition;
                verifyQueueNotOverflowed();
                item.IncreaseRefCounter();
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
                disposeQueue(WritePosition, lastIndex);

                //If cleared to end, clear from beginning to last read cursor too
                if (lastIndex == MaxQueueSize)
                {
                    lastIndex = readPositions
                        .Where(p => p < WritePosition)
                        .DefaultIfEmpty(0)
                        .Min();
                    disposeQueue(0, lastIndex);
                }

                //timer.Stop();
                //Trace.TraceInformation("Evicting old feed entries took {0} ms.", timer.ElapsedMilliseconds);
            }
        }

        private void disposeQueue(int firstIndex, int lastIndex)
        {
            for (var i = firstIndex; i < lastIndex; i++)
            {
                if (_queue[i] != null)
                {
                    var disposableItem = _queue[i] as IDisposable;
                    if (disposableItem != null)
                        disposableItem.Dispose();
                    _queue[i] = default(T);
                }
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
}
