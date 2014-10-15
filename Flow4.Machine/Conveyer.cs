using Flow4.IMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    class Conveyer : BaseMachineController, IConveyer
    {
        public Conveyer() : base(0)
        {

        }
    }
}
