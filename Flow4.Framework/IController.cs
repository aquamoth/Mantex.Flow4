using System;
using System.Threading.Tasks;
namespace Flow4.Framework
{
    public interface IController : IDisposable
    {
        bool IsAlive { get; }
        void Start();
        void Stop();
    }
}
