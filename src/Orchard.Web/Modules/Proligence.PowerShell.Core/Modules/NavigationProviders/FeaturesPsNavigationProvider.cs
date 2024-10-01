using Orchard.Environment.Extensions;
using Proligence.PowerShell.Core.Modules.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.NavigationProviders {
    public class FeaturesPsNavigationProvider : PsNavigationProvider {
        private readonly IExtensionManager _extensionManager;

        public FeaturesPsNavigationProvider(IExtensionManager extensionManager)
            : base(NodeType.Tenant) {
            _extensionManager = extensionManager;
        }

        protected override void InitializeInternal() {
            Node = new FeaturesNode(Vfs, _extensionManager);
        }
    }
}