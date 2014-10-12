using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Flow4.Entities;
using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Facilities.TypedFactory;

namespace Flow4.Sample.Castle.Windsor
{
    public class FrameworkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IFeedFactory>().ImplementedBy<FeedFactory>().LifestyleSingleton(),
                Classes.FromAssemblyContaining<IController>()
                    .Where(Component.IsInSameNamespaceAs<IController>())
                    .WithService
                    .DefaultInterfaces()
                    .LifestyleTransient()
                );

            //container.Kernel.Register(
            //    Component.For<IFeedFactory>().AsFactory(),
            //    Component.For<IFeed<IFrame>>().ImplementedBy<Feed<IFrame>>().LifeStyle.Transient
            //);
        }
    }
}
