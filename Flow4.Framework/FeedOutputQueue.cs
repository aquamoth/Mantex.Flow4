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
        internal int NextReadPosition { get; private set; }

        internal FeedOutputQueue(Feed<T> feed)
        {
            this._feed = feed;
            this.NextReadPosition = 0;
        }

        public void Dispose()
        {
            if (this._feed != null)
            {
                this._feed.Unsubscribe(this);
                this._feed = null;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this;
        }

        public bool IsEndOfFeed { get { return this.NextReadPosition == _feed.WritePosition; } }

        bool _itemRefCounted = false;
        bool IEnumerator.MoveNext()
        {
            if (this.IsEndOfFeed)
                return false;

            this.NextReadPosition++;
            if (this.NextReadPosition == _feed.MaxQueueSize)
                this.NextReadPosition = 0;

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
                var position = this.NextReadPosition - 1;
                if (position == -1)
                {
                    if (_feed.IsBeginningOfFeed)
                        throw new System.IO.EndOfStreamException("Beginning of stream. MoveNext first!");
                    position = _feed.MaxQueueSize - 1;
                }
                else if (position == 0)
                {
                    if (_feed.IsBeginningOfFeed)
                        throw new System.IO.EndOfStreamException("Beginning of stream. MoveNext first!");
                }
                else if (position == _feed.MaxQueueSize)
                {
                    position = 0;
                }

                if (!_itemRefCounted)
                {
                    _feed._queue[position].IncreaseRefCounter();
                    _itemRefCounted = true;
                }
                return _feed._queue[position];
            }
        }

        void IDisposable.Dispose()
        {
            //Do nothing. This occurs each time the foreach() statement has emptied the enumerator
        }

        object IEnumerator.Current { get { return (this as IEnumerator<T>).Current; } }

        void IEnumerator.Reset()
        {
            this.NextReadPosition = _feed.WritePosition;
        }

        public int Count { get { return _feed.WritePosition >= this.NextReadPosition ? _feed.WritePosition - this.NextReadPosition : _feed.MaxQueueSize - this.NextReadPosition + _feed.WritePosition; } }
    }
}
