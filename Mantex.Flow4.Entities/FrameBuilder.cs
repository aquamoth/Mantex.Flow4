using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class FrameBuilder : BaseRefCountedEntity, IBuilder<IFrame>, IFrame
    {
        public ICollection<IScanline> Lines { get; private set; }

        public FrameBuilder()
            : base()
        {
            Lines = new List<IScanline>();
        }

        IEnumerable<IScanline> IFrame.Lines { get { return Lines.AsEnumerable(); } }

        public IFrame Commit()
        {
            Lines = Lines.ToArray();
            return this as IFrame;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (this.ReferenceCounter == 0)
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
    }
}
