using Orchard.Data.Migration;
using Orchard.Environment.Extensions;
using Proligence.PowerShell.Core.Modules.Nodes;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.NavigationProviders {
    public class ThemesPsNavigationProvider : PsNavigationProvider {
        private readonly IExtensionManager _extensionManager;
        private readonly IDataMigrationManager _dataMigrationManager;

        public ThemesPsNavigationProvider(
            IExtensionManager extensionManager,
            IDataMigrationManager dataMigrationManager)
            : base(NodeType.Tenant) {
            _extensionManager = extensionManager;
            _dataMigrationManager = dataMigrationManager;
        }

        protected override void InitializeInternal() {
            Node = new ThemesNode(Vfs, _extensionManager, _dataMigrationManager);
        }
    }
}