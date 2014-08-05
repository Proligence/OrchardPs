namespace Proligence.PowerShell.Modules.NavigationProviders
{
    using Orchard.Data.Migration;
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="ThemesNode"/> node to the Orchard VFS.
    /// </summary>
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

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.Node = new ThemesNode(this.Vfs, this.extensionManager, this.dataMigrationManager);
        }
    }
}