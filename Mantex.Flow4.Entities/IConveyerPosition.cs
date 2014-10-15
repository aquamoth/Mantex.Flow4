using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities
{
    public interface IConveyerPosition : IRefCountedEntity
    {
        int Position { get; }
    }
}
