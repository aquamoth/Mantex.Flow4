﻿using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public interface IMachineController
    {
        State State { get; }
        Task<bool> Start();
        Task<bool> Stop();
    }
}