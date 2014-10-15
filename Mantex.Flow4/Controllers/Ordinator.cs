using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Ordinator
{
    public class Ordinator : IOrdinator, IDisposable
    {
        IExecutive executive;

        public Ordinator(IExecutive executive)
        {
            this.executive = executive;
            //TODO: Add references to Analysis, Monitors and Assessor
        }

        public void Start()
        {
            executive.Start().Wait();
        }

        public void Stop()
        {
            executive.Stop().Wait();
        }

        void IDisposable.Dispose()
        {
            if (executive is IDisposable)
                (executive as IDisposable).Dispose();
        }
    }
}
