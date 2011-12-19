using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Orchard.Environment;

namespace Orchard.Management.PsProvider.Agents {
    public class AgentBase {
        protected IContainer HostContainer { get; private set; }
        protected IContainerManager ContainerManager { get; private set; }

        public AgentBase() {
            HostContainer = OrchardStarter.CreateHostContainer(ContainerRegistrations);
            ContainerManager = HostContainer.Resolve<IContainerManager>();
        }

        public void Dispose() {
            if (HostContainer != null) {
                HostContainer.Dispose();
            }

            if (ContainerManager != null) {
                ContainerManager.Dispose();
            }
        }

        protected void ContainerRegistrations(ContainerBuilder builder) {
            // MVC Singletons
            builder.Register(ctx => RouteTable.Routes).SingleInstance();
            builder.Register(ctx => ModelBinders.Binders).SingleInstance();
            builder.Register(ctx => ViewEngines.Engines).SingleInstance();

            builder.RegisterType<ContainerManager>().As<IContainerManager>().SingleInstance();
        }
    }
}