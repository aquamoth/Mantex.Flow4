using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public abstract class BaseMachineController : IMachineController
    {
        readonly uint _heartbeatInterval;
        Timer heartbeat = null;
        object _timer_lock = new object();

        public State State { get; protected set; }

        public bool IsAlive { get { return heartbeat != null; } }

        public BaseMachineController(uint heartbeatInterval)
        {
            this._heartbeatInterval = heartbeatInterval;
            this.State = Machine.State.Stopped;
        }

        public virtual async Task<bool> Start()
        {
            if (State != Machine.State.Stopped)
                throw new NotSupportedException("Only stopped machine controllers can be started.");
            
            this.State = Machine.State.Starting;
            var success = await OnStart();
            if (success)
            {
                this.State = Machine.State.Running;
            }

            return success;
        }

        public virtual async Task<bool> Stop()
        {
            if (State != Machine.State.Running)
                throw new NotSupportedException("Only running machine controllers can be stopped.");

            this.State = Machine.State.Stopping;
            var success = await OnStop();
            if (success)
            {
                this.State = Machine.State.Stopped;
            }

            return success;
        }

        protected virtual async Task<bool> OnStart()
        {
            heartbeat = _heartbeatInterval > 0
                ? new Timer(new TimerCallback(Heartbeat), null, 0, _heartbeatInterval)
                : null;

            await Task.Delay(0);

            return true;
        }

        protected virtual async Task<bool> OnStop()
        {
            if (heartbeat == null)
                return false;

            var tmpHeartbeatTimer = heartbeat;
            heartbeat = null;

            await Task.Run(() =>
            {
                tmpHeartbeatTimer.Dispose();

                //Wait for the last heartbeat to complete
                //TODO: Should we enter an error state if this does not succeed?
                Monitor.TryEnter(_timer_lock, 15000);
            });

            return true;
        }

        #region Dispose Pattern

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (State == Machine.State.Running)
                    Stop().Wait();
            }
        }

        #endregion Dispose Pattern

        private void Heartbeat(object state)
        {
            if (!Monitor.TryEnter(_timer_lock))
            {
                //Something else has the lock, either a previous heartbeat or we are shutting down
                return;
            }
            try
            {
                OnHeartbeat();
            }
            finally
            {
                Monitor.Exit(_timer_lock);
            }
        }

        protected virtual void OnHeartbeat()
        {
        }
    }
}
