using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class ScanlinePool
    {
        #region Singleton pattern

        static object _lockObject = new object();
        static ScanlinePool _instance = null;
        public static ScanlinePool Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                lock (_lockObject)
                {
                    if (_instance == null)
                    {
                        _instance = new ScanlinePool();
                    }
                }
                return _instance;
            }
        }

        #endregion Singleton pattern

        Queue<ScanlineBuilder> freeObjects;

        int _numberOfPixels;
        public int NumberOfPixels
        {
            get
            {
                return _numberOfPixels;
            }
            set
            {
                if (_numberOfPixels != value)
                {
                    System.Diagnostics.Trace.Assert(freeObjects.Count == 0);
                    _numberOfPixels = value;
                }
            }
        }

        public int FreeObjectsCounter { get { return freeObjects.Count; } }

        private ScanlinePool()
        {
            freeObjects = new Queue<ScanlineBuilder>();
        }

        public IEnumerable<ScanlineBuilder> Take(int length)
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

        ScanlineBuilder[] takeFromPool(int maxCount)
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

        IEnumerable<ScanlineBuilder> createNew(int length)
        {
            if (length > 0)
            {
                //Trace.TraceInformation("Creating {0} new objects", length);
                for (var i = 0; i < length; i++)
                {
                    var builder = new ScanlineBuilder(_numberOfPixels);
                    builder.ReturnToPool += returnToPoolCallback;
                    //Debug.WriteLine("Created new object: " + builder.GetHashCode());
                    yield return builder;
                }
            }
        }

        private void returnToPoolCallback(object sender, EventArgs e)
        {
            //Return the poolable object to its pool
            var builder = (ScanlineBuilder)sender;
            builder.ReturnToPool -= returnToPoolCallback;
            lock(_lockObject)
            {
                freeObjects.Enqueue(builder);
            }
            //Debug.WriteLine("Returned object to pool: " + builder.GetHashCode());
        }

    }
}
