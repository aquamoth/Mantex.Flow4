using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.IMachine
{
    public interface IMachineController
    {
        State State { get; }

        event EventHandler StateChanged;

        Task<bool> Start();
        Task<bool> Stop();
    }
}
