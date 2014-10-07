using Flow4.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Marshaller : BaseController, IMarshaller
    {
        public Marshaller()
            : base(1000)
        {
        }

        public void Send(IFrame frame)
        {
            if (!IsAlive)
                throw new NotSupportedException("The marshaller is not alive.");

            //TODO: Put the object in a queue here to process on the next heartbeat
            Trace.TraceInformation("Killing off frame {0}", frame.Id);
            frame.Dispose();
        }
    }
}
