using System;
namespace Flow4.Sample.Controllers
{
    public interface IController : IDisposable
    {
        bool IsAlive { get; }
        void Start();
        void Stop();
    }
}
