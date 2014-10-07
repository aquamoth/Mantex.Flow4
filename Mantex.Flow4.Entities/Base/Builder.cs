using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public abstract class Builder<T> : BaseEntity
        where T : class, IEntity
    {
        public virtual T Commit()
        {
            return this as T;
        }
    }
}
