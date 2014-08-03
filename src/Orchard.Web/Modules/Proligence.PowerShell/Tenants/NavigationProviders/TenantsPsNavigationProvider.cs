namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="TenantsNode"/> node to the Orchard VFS.
    /// </summary>
    public class TenantsPsNavigationProvider : PsNavigationProvider
    {
        private readonly IShellSettingsManager manager;

        public TenantsPsNavigationProvider(IShellSettingsManager manager)
            : base(NodeType.Global)
        {
            this.manager = manager;
        }

        public override void Initialize()
        {
            this.Node = new TenantsNode(this.Vfs, this.manager);
        }
    }
}