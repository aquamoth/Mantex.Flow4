using Flow4.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Ordinator : BaseController
    {
        Executive executive;

        public Ordinator()
            : base(0)
        {

        }

        public override void Start()
        {
            base.Start();
            executive = new Executive();
            executive.Start();
        }

        public override void Stop()
        {
            executive.Stop();
            executive = null;
            base.Stop();
        }
    
    }
}
