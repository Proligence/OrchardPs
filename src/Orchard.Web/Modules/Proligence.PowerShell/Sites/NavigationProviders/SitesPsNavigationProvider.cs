// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitesPsNavigationProvider.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.NavigationProviders 
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Sites.Nodes;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="SitesNode"/> site node to the Orchard VFS.
    /// </summary>
    public class SitesPsNavigationProvider : PsNavigationProvider 
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
            this.Node = new SitesNode(this.Vfs, this.AgentManager.GetAgent<ITenantAgent>());
        }
    }
}