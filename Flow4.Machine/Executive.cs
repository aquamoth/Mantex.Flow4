using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Executive
    {
        IScanner scanner;
        IMarshaller marshaller;

        public void Start()
        {
            scanner = new Scanner();
            marshaller = new Marshaller();

            scanner.FrameCreated += (sender, e) =>
            {
                marshaller.Send(e.Frame);
            };

            marshaller.Start();
            scanner.Start();
        }

        public void Stop()
        {
            scanner.Stop();
            marshaller.Stop();
        }
    }
}
