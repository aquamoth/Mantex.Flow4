using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class FrameBuilder : Builder<IFrame>, IFrame, IDisposable
    {
        public ICollection<IScanline> Lines { get; private set; }

        public FrameBuilder() 
            : base()
        {
            Lines = new List<IScanline>();
        }

        ~FrameBuilder()
        {
            System.Diagnostics.Trace.TraceInformation("FrameBuilder finalizer for {0}", Id);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var scanline in Lines)
                {
                    scanline.Dispose();
                }
                Lines = null;
            }
            Trace.TraceInformation("AFTER Disposing: {0} allocated and {1} free objects.", ScanlinePool.Instance.PerformanceIndex.Item2, ScanlinePool.Instance.PerformanceIndex.Item1);
        }
    }
}
