using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using Castle.Windsor.Installer;
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
            var container = new WindsorContainer();
            container.Kernel.AddFacility<TypedFactoryFacility>();
            container.Install(FromAssembly.This());

            var ordinator = container.Resolve<IOrdinator>();
            ordinator.Start();

            Console.ReadLine();

            ordinator.Stop();
            container.Dispose();
        }
    }

}
