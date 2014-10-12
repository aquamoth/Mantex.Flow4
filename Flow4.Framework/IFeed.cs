using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Framework
{
    public interface IFeed : IDisposable
    {
    }

    public interface IFeed<T> : IFeed, IController
        where T : IRefCountedEntity
    {
        FeedOutputQueue<T> Subscribe();
        void Enqueue(T item);
    }
}
