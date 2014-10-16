using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public abstract class Pool<T> : IPool<T>
        where T : IPoolable
    {
        object _lockObject = new object();
        protected Queue<T> freeObjects;

        public virtual int FreeObjectsCounter { get { return freeObjects.Count; } }

        public Pool()
        {
            freeObjects = new Queue<T>();
        }

        public virtual IEnumerable<T> Take(int length)
        {
            //Trace.TraceInformation("BEFORE: Pool size: {0} allocated and {1} free objects.", allocatedObjects.Count, freeObjects.Count);

            //Return existing items from the free pool
            var pooledItems = takeFromPool(length);
            var countFromPool = pooledItems.Count();
            foreach (var item in pooledItems)
                yield return item;

            var newItems = createNew(length - countFromPool);
            foreach (var item in newItems)
                yield return item;

            //Trace.TraceInformation("AFTER : Pool size: {0} allocated and {1} free objects.", allocatedObjects.Count, freeObjects.Count);
        }

        protected virtual T[] takeFromPool(int maxCount)
        {
            lock (_lockObject)
            {
                var length = Math.Min(maxCount, freeObjects.Count);
                //Trace.TraceInformation("Fetching {0} objects from pool.", length);
                return Enumerable
                    .Repeat(0, length)
                    .Select(x =>
                    {
                        var builder = freeObjects.Dequeue();
                        builder.ReturnToPool += returnToPoolCallback;
                        return builder;
                    }).ToArray();
            }
        }

        IEnumerable<T> createNew(int length)
        {
            if (length > 0)
            {
                //Trace.TraceInformation("Creating {0} new objects", length);
                for (var i = 0; i < length; i++)
                {
                    var item = OnCreateNew();
                    item.ReturnToPool += returnToPoolCallback;
                    yield return item;
                }
            }
        }

        protected abstract T OnCreateNew();

        private void returnToPoolCallback(object sender, EventArgs e)
        {
            //Return the poolable object to its pool
            var builder = (T)sender;
            builder.ReturnToPool -= returnToPoolCallback;
            lock (_lockObject)
            {
                freeObjects.Enqueue(builder);
            }
            //Debug.WriteLine("Returned object to pool: " + builder.GetHashCode());
            //Trace.TraceInformation("AFTER Disposing: {0} free objects.", this.FreeObjectsCounter);
        }
    }
}
