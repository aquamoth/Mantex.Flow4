using Flow4.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class FrameCreatedEventArgs : EventArgs
    {
        public IFrame Frame { get; set; }
    }
}
