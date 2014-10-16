using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Scanner : BaseMachineController, IScanner
    {
        public Scanner() 
            : base(100)
        {
        }

        public IDetector Detector { get; set; }
        public IXray Xray { get; set; }

        protected override async Task<bool> OnStart()
        {
            if (Xray == null)
                throw new ApplicationException("Xray not defined.");
            if (Detector == null)
                throw new ApplicationException("Detector not defined.");

            this.MonitoredControllers.Add(Xray);
            this.MonitoredControllers.Add(Detector);

            return await base.OnStart();
        }

        protected override async Task<bool> OnStop()
        {
            var success = await base.OnStop();

            this.MonitoredControllers.Remove(Xray);
            this.MonitoredControllers.Remove(Detector);
            
            return success;
        }
    }
}
