namespace Proligence.PowerShell.Modules.NavigationProviders
{
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ThemesPsNavigationProvider : PsNavigationProvider
    {
        private readonly IExtensionManager extensionManager;
        private readonly IDataMigrationManager dataMigrationManager;

        public ThemesPsNavigationProvider(
            IExtensionManager extensionManager,
            IDataMigrationManager dataMigrationManager)
            : base(NodeType.Tenant)
        {
            this.extensionManager = extensionManager;
            this.dataMigrationManager = dataMigrationManager;
        }

        public override void Initialize()
        {
            this.Node = new ThemesNode(this.Vfs, this.extensionManager, this.dataMigrationManager);
        }
    }
}