using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class BaseController : IController, IDisposable
    {
        readonly uint _heartbeatInterval;
        Timer heartbeat = null;

        public BaseController(uint heartbeatInterval)
        {
            this._heartbeatInterval = heartbeatInterval;
        }

        public virtual void Start()
        {
            heartbeat = _heartbeatInterval > 0
                ? new Timer(new TimerCallback(OnHeartbeat), null, 0, _heartbeatInterval)
                : null;
        }

        public virtual void Stop()
        {
            if (heartbeat != null)
            {
                heartbeat.Dispose();
                heartbeat = null;

                //TODO: Await completion of the current heartbeat before returning
                //Debug.WriteLine("Waiting for heartbeat to complete.");
                //Thread.Sleep(3000);
            }
        }

        public bool IsAlive { get { return heartbeat != null; } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsAlive)
                Stop();
        }

        protected virtual void OnHeartbeat(object state)
        {
        }
    }
}
