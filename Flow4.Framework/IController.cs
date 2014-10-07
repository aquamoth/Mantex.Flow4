using System;
namespace Flow4.Sample.Controllers
{
    public interface IController
    {
        bool IsAlive { get; }
        void Start();
        void Stop();
    }
}
