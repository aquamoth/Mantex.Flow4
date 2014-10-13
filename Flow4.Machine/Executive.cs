using Flow4.Framework;
using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Executive : BaseMachineController, IExecutive
    {
        IScanner scanner;
        IMarshaller marshaller;

        public Executive(IScanner scanner, IMarshaller marshaller)
            : base(0)
        {
            this.scanner = scanner;
            this.marshaller = marshaller;
        }

        protected override async Task<bool> OnStart()
        {
            var success = await marshaller.Start();
            if (!success)
                return false;
            success = await scanner.Start();
            if (!success)
            {
                await marshaller.Stop();
                return false;
            }
            success = await base.OnStart();
            if (!success)
            {
                await scanner.Stop();
                await marshaller.Stop();
                return false;
            }
            return true;
        }

        protected override async Task<bool> OnStop()
        {
            var success = await scanner.Stop();
            success &= await marshaller.Stop();
            success &= await base.Stop();
            return success;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (marshaller is IDisposable)
                    (marshaller as IDisposable).Dispose();
                if (scanner is IDisposable)
                    (scanner as IDisposable).Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
