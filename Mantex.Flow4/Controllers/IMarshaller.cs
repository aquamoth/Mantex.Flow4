using System;
namespace Flow4.Sample.Controllers
{
    interface IMarshaller : IController
    {
        void Send(Flow4.Entities.IFrame frame);
    }
}
