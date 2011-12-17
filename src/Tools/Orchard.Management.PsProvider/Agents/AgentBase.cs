using System.Diagnostics.CodeAnalysis;
using Autofac;
using Orchard.Environment;

namespace Orchard.Management.PsProvider.Agents {
    public class AgentBase {
        protected IContainer HostContainer { get; private set; }

        public AgentBase() {
            HostContainer = CreateHostContainer();
        }

        public void Dispose() {
            if (HostContainer != null) {
                HostContainer.Dispose();
            }
        }

        protected void ContainerRegistrations(ContainerBuilder builder) {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private IContainer CreateHostContainer() {
            return OrchardStarter.CreateHostContainer(ContainerRegistrations);
        }
    }
}