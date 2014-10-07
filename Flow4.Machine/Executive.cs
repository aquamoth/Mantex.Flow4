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
        Scanner scanner;
        Marshaller marshaller;

        public Executive(HashSet<object> resources)
        {
            scanner = new Scanner(resources);
            marshaller = new Marshaller(resources);
        }

        public void Start()
        {
            //scanner.FrameCreated += (sender, e) =>
            //{
            //    marshaller.Send(e.Frame);
            //};

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
