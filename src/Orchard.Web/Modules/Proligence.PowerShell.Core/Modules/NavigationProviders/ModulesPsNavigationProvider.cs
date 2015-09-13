using Orchard.Environment.Extensions;
using Proligence.PowerShell.Core.Modules.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.NavigationProviders {
    public class ModulesPsNavigationProvider : PsNavigationProvider {
        private readonly IExtensionManager _extensionManager;

        public ModulesPsNavigationProvider(IExtensionManager extensionManager)
            : base(NodeType.Tenant) {
            _extensionManager = extensionManager;
        }

        protected override void InitializeInternal() {
            Node = new ModulesNode(Vfs, _extensionManager);
        }
    }
}