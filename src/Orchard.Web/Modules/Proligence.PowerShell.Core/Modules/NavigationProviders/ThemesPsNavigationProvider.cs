namespace Proligence.PowerShell.Core.Modules.NavigationProviders
{
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Core.Modules.Nodes;
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

        protected override void InitializeInternal()
        {
            this.Node = new ThemesNode(this.Vfs, this.extensionManager, this.dataMigrationManager);
        }
    }
}