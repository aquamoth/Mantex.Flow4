using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Entities.Base
{
    public interface IEntity
    {
        Guid Id { get; }
        DateTime Created { get; }
    }
}
