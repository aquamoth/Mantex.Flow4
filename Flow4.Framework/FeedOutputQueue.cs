using Flow4.Entities.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Framework
{
    public class FeedOutputQueue<T> : IEnumerable<T>, IEnumerator<T>, IDisposable
        where T : IRefCountedEntity
    {
        Feed<T> _feed;
        internal int CurrentPosition { get; private set; }

        internal FeedOutputQueue(Feed<T> feed)
        {
            this._feed = feed;
            this.CurrentPosition = -1;
        }

        public void Dispose()
        {
            this._feed.Unsubscribe(this);
            this._feed = null;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        public bool IsEndOfFeed { get { return this.CurrentPosition == _feed.WritePosition; } }

        bool _itemRefCounted = false;
        bool IEnumerator.MoveNext()
        {
            if (((this.CurrentPosition + 1) % _feed.MaxQueueSize) == _feed.WritePosition)
                return false;

            this.CurrentPosition++;
            if (this.CurrentPosition == _feed.MaxQueueSize)
                this.CurrentPosition = 0;

            _itemRefCounted = false;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        T IEnumerator<T>.Current
        {
            get
            {
                if (this.CurrentPosition == -1)
                    throw new System.IO.EndOfStreamException("Beginning of stream. MoveNext first!");
                if (IsEndOfFeed)
                    throw new System.IO.EndOfStreamException();

                if (!_itemRefCounted)
                {
                    _feed._queue[this.CurrentPosition].IncreaseRefCounter();
                    _itemRefCounted = true;
                }
                return _feed._queue[this.CurrentPosition];
            }
        }

        void IDisposable.Dispose()
        {
            //Do nothing. This occurs each time the foreach() statement has emptied the enumerator
        }

        object IEnumerator.Current { get { return (this as IEnumerator<T>).Current; } }

        void IEnumerator.Reset()
        {
            this.CurrentPosition = _feed.WritePosition;
        }

        public int Count { get { return _feed.WritePosition >= this.CurrentPosition ? _feed.WritePosition - this.CurrentPosition : _feed.MaxQueueSize - this.CurrentPosition + _feed.WritePosition; } }
    }
}
