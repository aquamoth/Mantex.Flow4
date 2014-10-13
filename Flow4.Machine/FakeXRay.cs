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
        public FakeXRay()
            : base(1000)
        {
        }

        protected override async Task<bool> OnStart()
        {
            await Task.Delay(5000);
            //await base.OnStart();
            return true;
        }

        protected override async Task<bool> OnStop()
        {
            //await base.OnStop();
            await Task.Delay(1000);
            return true;
        }
    }
}
