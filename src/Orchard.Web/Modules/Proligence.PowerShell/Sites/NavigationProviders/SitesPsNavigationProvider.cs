// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitesPsNavigationProvider.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.NavigationProviders 
{
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Sites.Nodes;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="SitesNode"/> site node to the Orchard VFS.
    /// </summary>
    public class SitesPsNavigationProvider : PsNavigationProvider 
    {
        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            NodeType = NodeType.Global;
            Node = new SitesNode(Vfs, AgentManager.GetAgent<TenantAgentProxy>());
        }
    }
}