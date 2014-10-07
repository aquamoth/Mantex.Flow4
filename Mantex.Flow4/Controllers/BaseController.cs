using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class BaseController : Flow4.Sample.Controllers.IController
    {
        readonly uint _heartbeatInterval;
        Timer heartbeat = null;

        public BaseController(uint heartbeatInterval)
        {
            this._heartbeatInterval = heartbeatInterval;
        }

        public virtual void Start()
        {
            heartbeat = new Timer(new TimerCallback(OnHeartbeat), null, 0, _heartbeatInterval);
        }

        public virtual void Stop()
        {
            heartbeat.Dispose();
            heartbeat = null;
            //TODO: Await completion of the current heartbeat before returning
            Debug.WriteLine("Waiting for heartbeat to complete.");
            Thread.Sleep(3000);
        }

        public bool IsAlive { get { return heartbeat != null; } }

        protected virtual void OnHeartbeat(object state)
        {
        }
    }
}
