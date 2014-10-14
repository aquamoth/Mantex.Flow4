using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public bool IsAlive { get { return heartbeat != null; } }

        #region Machine State

        State _state;
        public State State
        {
            get { return _state; }
            protected set
            {
                if (_state != value)
                {
                    _state = value;
                    OnStateChanged(EventArgs.Empty);
                }
            }
        }

        public event EventHandler StateChanged;
        protected virtual void OnStateChanged(EventArgs e)
        {
            if (StateChanged != null)
                StateChanged(this, e);
        }

        #endregion Machine State

        protected ICollection<IMachineController> MonitoredControllers { get; set; }

        public BaseMachineController(uint heartbeatInterval)
        {
            this._heartbeatInterval = heartbeatInterval;
            this.State = Machine.State.Stopped;
            MonitoredControllers = createMonitoredCollection();
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
            switch (State)
            {
                case State.NotReady: return false;
                case State.Stopped: return true;
                case State.Starting:
                    throw new NotSupportedException("Not yet supporting stopping a Starting controller.");

                case State.Running:
                    this.State = Machine.State.Stopping;
                    var success = await OnStop();
                    if (success)
                    {
                        this.State = Machine.State.Stopped;
                    }
                    return success;

                case State.Stopping:
                    Task.Delay(5000).Wait();//TODO: Rewrite better, so we only wait for the state to change
                    return this.State == Machine.State.Stopped;

                case State.Failure: return true;

                default: 
                    throw new NotSupportedException();
            }
        }

        protected virtual async Task<bool> OnStart()
        {
            var success = OnVerifyStateMonitoredControllers(State.Stopped);
            if (!success)
                return false;

            success = await OnStartMonitoredControllers();
            if (!success)
            {
                success = await OnStopMonitoredControllers();
                return false;
            }

            heartbeat = _heartbeatInterval > 0
                ? new Timer(new TimerCallback(Heartbeat), null, 0, _heartbeatInterval)
                : null;

            await Task.Delay(0);

            return true;
        }

        protected virtual async Task<bool> OnStop()
        {
            var success = await OnStopMonitoredControllers();

            if (heartbeat != null)
            {
                var tmpHeartbeatTimer = heartbeat;
                heartbeat = null;
                await Task.Run(() =>
                {
                    tmpHeartbeatTimer.Dispose();

                    //Wait for the last heartbeat to complete
                    //TODO: Should we enter an error state if this does not succeed?
                    Monitor.TryEnter(_timer_lock, 15000);
                });
            }

            return success;
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

        #region Monitored Controllers
        
        protected virtual bool OnVerifyStateMonitoredControllers(Machine.State state)
        {
            return MonitoredControllers.All(c => c.State == state);
        }

        protected virtual async Task<bool> OnStartMonitoredControllers()
        {
            var success = await Task.WhenAll(MonitoredControllers.Select(c => c.Start()));
            return success.All(b => b);
        }

        protected virtual async Task<bool> OnStopMonitoredControllers()
        {
            var success = await Task.WhenAll(MonitoredControllers.Select(c => c.Stop()));
            return success.All(b => b);
        }

        void monitoredController_StateChanged(object sender, EventArgs e)
        {
            if ((sender as IMachineController).State == Machine.State.Failure)
            {
                Trace.TraceError("Detected failure of a monitored machine controller!");
                this.State = Machine.State.Failure;

                Task.WhenAll(MonitoredControllers.Select(c => c.Stop())).Wait();
            }
        }

        ObservableCollection<IMachineController> createMonitoredCollection()
        {
            var collection = new ObservableCollection<IMachineController>();
            collection.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        if (e.OldItems != null)
                            foreach (IMachineController item in e.OldItems)
                                item.StateChanged -= monitoredController_StateChanged;
                        if (e.NewItems != null)
                            foreach (IMachineController item in e.NewItems)
                                item.StateChanged += monitoredController_StateChanged;
                        break;
                }
            };
            return collection;
        }

        #endregion Monitored Controllers

        #region Heartbeat

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

        #endregion Heartbeat

    }
}
