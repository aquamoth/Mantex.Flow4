using System;
namespace Flow4.Entities.Base
{
    public interface IPool<T>
     where T : IPoolable
    {
        int FreeObjectsCounter { get; }
        System.Collections.Generic.IEnumerable<T> Take(int length);
    }
}
