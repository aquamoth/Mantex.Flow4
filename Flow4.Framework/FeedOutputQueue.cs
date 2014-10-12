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
        internal int ReadPosition { get; private set; }

        internal FeedOutputQueue(Feed<T> feed)
        {
            this._feed = feed;
            this.ReadPosition = -1;
        }

        public void Dispose()
        {
            this._feed.Unsubscribe(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        bool _itemRefCounted = false;
        bool IEnumerator.MoveNext()
        {
            if (_feed.WritePosition == -1 || this.ReadPosition + 1 == _feed.WritePosition)
                return false;

            this.ReadPosition++;
            if (this.ReadPosition == _feed.MaxQueueSize)
                this.ReadPosition = 0;

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
                if (this.ReadPosition == -1)
                    throw new System.IO.EndOfStreamException("Beginning of stream. MoveNext first!");
                if (this.ReadPosition == _feed.WritePosition)
                    throw new System.IO.EndOfStreamException();

                if (!_itemRefCounted)
                {
                    _feed._queue[this.ReadPosition].IncreaseRefCounter();
                    _itemRefCounted = true;
                }
                return _feed._queue[this.ReadPosition];
            }
        }

        void IDisposable.Dispose()
        {
            //Do nothing. This occurs each time the foreach() statement has emptied the enumerator
        }

        object IEnumerator.Current { get { return (this as IEnumerator<T>).Current; } }

        void IEnumerator.Reset()
        {
            this.ReadPosition = _feed.WritePosition;
        }

        public int Count { get { return _feed.WritePosition >= this.ReadPosition ? _feed.WritePosition - this.ReadPosition : _feed.MaxQueueSize - this.ReadPosition + _feed.WritePosition; } }
    }
}
