using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class FakeXRay : IXray
    {
        public State State { get; protected set; }

        public FakeXRay()
            //: base(1000)
        {
            this.State = Machine.State.Stopped;
        }

        public async Task<bool> Start()
        {
            if (State != Machine.State.Stopped)
                throw new NotSupportedException("Only stopped machine controllers can be started.");

            this.State = Machine.State.Starting;
            
            await Task.Delay(5000);

            this.State = Machine.State.Running;
            return true;
        }

        public async Task<bool> Stop()
        {
            if (State != Machine.State.Running)
                throw new NotSupportedException("Only running machine controllers can be stopped.");

            this.State = Machine.State.Stopping;

            await Task.Delay(1000);

            this.State = Machine.State.Stopped;

            return true;
        }
    }
}
