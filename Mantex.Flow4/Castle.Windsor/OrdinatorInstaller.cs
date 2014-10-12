using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Castle.Windsor
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
