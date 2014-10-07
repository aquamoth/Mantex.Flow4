using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public abstract class BaseEntity : IEntity
    {
        public Guid Id { get; protected set; }
        public DateTime Created { get; protected set; }

        public BaseEntity()
        {
            Id = Guid.NewGuid();
            Created = DateTime.Now;
        }
    }

    public abstract class DisposableEntity : BaseEntity, IDisposable
    {
        ~DisposableEntity()
        {
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {

        }
    }

    public interface IPoolable : IDisposable
    {
        event EventHandler ReturnToPool;
    }

    public abstract class PoolableEntity : DisposableEntity, IPoolable
    {
        public event EventHandler ReturnToPool;
        protected void OnReturnToPool(EventArgs e)
        {
            if (ReturnToPool != null)
                ReturnToPool(this, e);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                OnReturnToPool(EventArgs.Empty);
            }
            else if (ReturnToPool != null)
            {
                throw new NotSupportedException("A poolable object was garbage collected instead of returned to its pool.");
            }
        }
    }

}
