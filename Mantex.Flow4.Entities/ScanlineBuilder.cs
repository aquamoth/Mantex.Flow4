using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class ScanlineBuilder : Builder<IScanline>, IScanline, IPoolable
    {
        public byte[] Pixels { get; private set; }

        public ScanlineBuilder(int numberOfPixels)
            :base()
        {
            Pixels = new byte[numberOfPixels];
        }

        ~ScanlineBuilder()
        {
            //System.Diagnostics.Trace.TraceInformation("Scanline finalizer for {0}", Id);
            if (ReturnToPool != null)
            {
                Trace.TraceError("Scanline finalizer called for {0} when expecting to Dispose() back to pool!", Id);
                //throw new ApplicationException("ScanlineBuilder finalized when expected to be returned to a pool!");
            }
        }

        IEnumerable<byte> IScanline.Pixels { get { return Pixels.AsEnumerable(); } }





        public event EventHandler ReturnToPool;

        void IDisposable.Dispose()
        {
            if (ReturnToPool != null)
                ReturnToPool(this, EventArgs.Empty);
            //GC.SuppressFinalize(this);
        }
    }
}
