using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Conveyer : BaseMachineController, IConveyer
    {
        IFeed<IConveyerPosition> conveyerPositionFeed;

        public Conveyer(IFeedFactory feedFactory) 
            : base(100)
        {
            conveyerPositionFeed = feedFactory.GetFeedOf<IConveyerPosition>("ConveyerPositionFeed");
        }

        protected override void OnHeartbeat()
        {
            base.OnHeartbeat();

        }
    }
}
