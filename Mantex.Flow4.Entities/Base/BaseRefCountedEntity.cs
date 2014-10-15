using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public abstract class BaseRefCountedEntity : BaseEntity, IRefCountedEntity
    {
        protected int ReferenceCounter = 1;

        ~BaseRefCountedEntity()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            if (--ReferenceCounter == 0)
            {
                Dispose(true);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void IncreaseRefCounter() { ReferenceCounter++; }
    }
}
