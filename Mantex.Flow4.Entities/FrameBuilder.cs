using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
//TODO: Base on BaseRefCountedEntity instead of "stupid" Builder<>
    public class FrameBuilder : Builder<IFrame>, IFrame, IDisposable
    {
        int _referenceCounter = 1;

        public ICollection<IScanline> Lines { get; private set; }

        public FrameBuilder() 
            : base()
        {
            Lines = new List<IScanline>();
            //System.Diagnostics.Trace.TraceInformation("FrameBuilder created for {0}", Id);
        }

        ~FrameBuilder()
        {
            System.Diagnostics.Trace.TraceWarning("FrameBuilder finalizer for {0}", Id);
            Dispose(false);
        }

        IEnumerable<IScanline> IFrame.Lines { get { return Lines.AsEnumerable(); } }

        public override IFrame Commit() 
        {
            Lines = Lines.ToArray();
            return base.Commit();
        }

        public void Dispose()
        {
            //System.Diagnostics.Trace.TraceInformation("FrameBuilder Dispose for {0}", Id);
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (--_referenceCounter == 0)
                {
                    foreach (var scanline in Lines)
                    {
                        scanline.Dispose();
                    }
                    Lines = null;
                    Trace.TraceInformation("AFTER Disposing: {0} free objects.", ScanlinePool.Instance.FreeObjectsCounter);
                }
            }
        }

        public void IncreaseRefCounter() { _referenceCounter++; }
    }
}
