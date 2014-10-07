using System;
namespace Flow4.Sample.Controllers
{
    interface IController
    {
        bool IsAlive { get; }
        void Start();
        void Stop();
    }
}
