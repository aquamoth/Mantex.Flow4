using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow4.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            IScanner scanner = new Controllers.Scanner();
            IMarshaller marshaller = new Controllers.Marshaller();

            scanner.FrameCreated += (sender, e) =>
            {
                marshaller.Send(e.Frame);
            };

            marshaller.Start();
            scanner.Start();

            Console.ReadLine();
            
            scanner.Stop();
            marshaller.Stop();
        }
    }

}
