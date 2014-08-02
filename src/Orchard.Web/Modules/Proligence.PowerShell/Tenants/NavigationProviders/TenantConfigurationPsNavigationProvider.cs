namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="TenantConfigurationNode"/> to each tenant node
    /// in the Orchard VFS.
    /// </summary>
    public class TenantConfigurationPsNavigationProvider : PsNavigationProvider 
    {
        private readonly ITenantAgent agent;

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Tenant;
            this.Node = new TenantConfigurationNode(this.Vfs, this.agent);
        }
    }
}