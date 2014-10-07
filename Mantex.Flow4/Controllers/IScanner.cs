using System;
namespace Flow4.Sample.Controllers
{
    interface IScanner : IController
    {
        event EventHandler<FrameCreatedEventArgs> FrameCreated;
    }
}
