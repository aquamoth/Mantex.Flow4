using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public interface IBuilder<T>
        where T : class, IEntity
    {
        T Commit();
    }

    public abstract class Builder<T> : BaseEntity, IBuilder<T>
        where T : class, IEntity
    {
        public virtual T Commit()
        {
            return this as T;
        }
    }
}
