namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="TenantsNode"/> node to the Orchard VFS.
    /// </summary>
    public class TenantsPsNavigationProvider : PsNavigationProvider 
    {
        private readonly ITenantAgent agent;

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Global;
            this.Node = new TenantsNode(this.Vfs, this.agent);
        }
    }
}