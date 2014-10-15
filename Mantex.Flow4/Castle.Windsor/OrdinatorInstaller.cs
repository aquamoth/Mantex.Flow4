using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Flow4.Ordinator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Ordinator.Castle.Windsor
{
    public class OrdinatorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromThisAssembly()
                    .Where(Component.IsInSameNamespaceAs<Ordinator>())
                    .WithService
                    .DefaultInterfaces()
                    .LifestyleTransient()
                );
        }
    }
}
