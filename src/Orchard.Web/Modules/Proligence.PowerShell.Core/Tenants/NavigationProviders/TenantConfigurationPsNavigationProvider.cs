using Proligence.PowerShell.Core.Tenants.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Tenants.NavigationProviders {
    public class TenantConfigurationPsNavigationProvider : PsNavigationProvider {
        public TenantConfigurationPsNavigationProvider()
            : base(NodeType.Tenant) {
        }

        protected override void InitializeInternal() {
            Node = new TenantConfigurationNode(Vfs);
        }
    }
}