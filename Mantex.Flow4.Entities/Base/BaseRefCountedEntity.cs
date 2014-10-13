using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public abstract class BaseRefCountedEntity : BaseEntity, IRefCountedEntity
    {
        int _referenceCounter = 1;

        ~BaseRefCountedEntity()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (--_referenceCounter == 0)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void IncreaseRefCounter() { _referenceCounter++; }
    }
}
