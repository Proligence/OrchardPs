using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Vfs.Navigation;
using Proligence.PowerShell.Provider.Vfs.Nodes;

namespace Proligence.PowerShell.Provider.Vfs.NavigationProviders {
    public class TenantsPsNavigationProvider : PsNavigationProvider {
        private readonly IShellSettingsManager _shellSettingsManager;

        public TenantsPsNavigationProvider(IShellSettingsManager shellSettingsManager)
            : base(NodeType.Global) {
            _shellSettingsManager = shellSettingsManager;
        }

        protected override void InitializeInternal() {
            Node = new TenantsNode(Vfs, _shellSettingsManager);
        }
    }
}