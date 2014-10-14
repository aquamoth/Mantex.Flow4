using Flow4.Entities;
using Flow4.Framework;
using Flow4.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Ordinator : IOrdinator, IDisposable
    {
        IExecutive executive;

        public Ordinator(IExecutive executive)
        {
            this.executive = executive;
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
