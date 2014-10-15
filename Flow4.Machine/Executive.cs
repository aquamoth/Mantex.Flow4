using Flow4.Framework;
using Flow4.IMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Executive : BaseMachineController, IExecutive
    {
        public Executive(IScanner scanner, IMarshaller marshaller)
            : base(0)
        {
            this.MonitoredControllers.Add(scanner);
            this.MonitoredControllers.Add(marshaller);
            //TODO: Add references to AUX, Climate and Housing
        }
    }
}
