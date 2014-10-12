using System;
using Flow4.Entities;
using Flow4.Entities.Base;

namespace Flow4.Framework
{
    public interface IFeedFactory : IDisposable
    {
        IFeed<T> GetFeedOf<T>(string name) where T : IRefCountedEntity;
        //void Destroy(IFeed<IFrame> feed);
    }
}
