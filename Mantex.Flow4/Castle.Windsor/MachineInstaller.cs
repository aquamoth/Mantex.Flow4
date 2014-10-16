using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Flow4.IMachine;
using Flow4.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Castle.Windsor
{
    public class MachineInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Install(Configuration.FromAppConfig());
            //container.Register(
            //    Component.For<IXray>()
            //        .ImplementedBy<FakeXRay>()
            //        .LifestyleTransient()
            //    );

            container.Register(
                Classes.FromAssemblyContaining<Executive>()
                    .Where(Component.IsInSameNamespaceAs<Executive>())
                    .WithService
                    .AllInterfaces()
                    .LifestyleTransient()
                );
        }
    }
}
