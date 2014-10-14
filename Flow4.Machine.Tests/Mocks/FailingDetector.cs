using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine.Tests.Mocks
{
    class FailingDetector: BaseMachineController, IDetector
    {
        DateTime startTime = DateTime.MaxValue;

        public FailingDetector() : base(500) { }

        protected override async Task<bool> OnStart()
        {
            var success = await base.OnStart();
            if (success)
                startTime = DateTime.Now;
            return success;
        }

        protected override void OnHeartbeat()
        {
            base.OnHeartbeat();
            if (DateTime.Now >= startTime.AddSeconds(1))
                this.State = Machine.State.Failure;
        }

    }
}
