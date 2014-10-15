using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.IMachine
{
    public enum State
    {
        NotReady = 0,

        Stopped = 1,
        Starting = 3,
        Running = 4,
        Stopping = 5,

        Failure = 64
    }
}
