using System.Management.Automation;
using Autofac;

namespace Orchard.Management.PsProvider {
    public class OrchardProviderInfo : ProviderInfo {
        internal OrchardProviderInfo(ProviderInfo providerInfo, IContainer container) : base(providerInfo) {
            Container = container;
        }

        internal IContainer Container { get; private set; }
    }
}