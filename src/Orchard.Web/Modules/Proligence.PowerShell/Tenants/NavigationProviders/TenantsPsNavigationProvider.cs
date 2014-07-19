namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Tenants.Nodes;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="TenantsNode"/> node to the Orchard VFS.
    /// </summary>
    public class TenantsPsNavigationProvider : PsNavigationProvider 
    {
        /// <summary>
        /// Gets or sets the Orchard agent manager.
        /// </summary>
        public IAgentManager AgentManager { get; set; }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Global;
            this.Node = new TenantsNode(this.Vfs, this.AgentManager.GetAgent<ITenantAgent>());
        }
    }
}