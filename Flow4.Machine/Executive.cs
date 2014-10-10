using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Executive : BaseController
    {
        Scanner scanner;
        Marshaller marshaller;

        public Executive(HashSet<object> resources)
            : base(0)
        {
            scanner = new Scanner(resources);
            marshaller = new Marshaller(resources);
        }

        public void Start()
        {
            marshaller.Start();
            scanner.Start();
        }

        public void Stop()
        {
            scanner.Stop();
            marshaller.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing)
            {
                marshaller.Dispose();
                scanner.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
