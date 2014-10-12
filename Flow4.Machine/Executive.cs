using Flow4.Framework;
using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Executive : BaseController, IExecutive
    {
        IScanner scanner;
        IMarshaller marshaller;

        public Executive(IScanner scanner, IMarshaller marshaller /*, HashSet<object> resources*/)
            : base(0)
        {
            //HashSet<object> resources = new HashSet<object>();
            this.scanner = scanner;// new Scanner(resources);
            this.marshaller = marshaller;// new Marshaller(resources);
        }

        //void IDisposable.Dispose()
        //{
        //    Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        public override void Start()
        {
            base.Start();
            marshaller.Start();
            scanner.Start();
        }

        public override void Stop()
        {
            scanner.Stop();
            marshaller.Stop();
            base.Stop();
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
