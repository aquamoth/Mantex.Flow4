using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public class ConveyerPositionBuilder : BaseRefCountedEntity, IBuilder<IConveyerPosition>, IConveyerPosition
    {
        public int Value { get; set; }

        public IConveyerPosition Commit()
        {
            return this as IConveyerPosition;
        }
    }
}
