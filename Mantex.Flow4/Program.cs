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
            var ordinator = new Ordinator();
            ordinator.Start();

            Console.ReadLine();

            ordinator.Stop();
        }
    }

}
