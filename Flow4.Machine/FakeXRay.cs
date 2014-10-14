using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class FakeXRay : BaseMachineController, IXray
    {
        readonly int _startupTime;
        readonly int _shutdownTime;

        public FakeXRay(int startupTime = 5000, int shutdownTime = 1000)
            : base(1000)
        {
            this._startupTime = startupTime;
            this._shutdownTime = shutdownTime;
        }

        protected override async Task<bool> OnStart()
        {
            await Task.Delay(_startupTime);
            //await base.OnStart();
            return true;
        }

        protected override async Task<bool> OnStop()
        {
            //await base.OnStop();
            await Task.Delay(_shutdownTime);
            return true;
        }
    }
}
