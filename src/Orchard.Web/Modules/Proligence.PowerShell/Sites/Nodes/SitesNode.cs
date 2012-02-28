// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitesNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Sites.Items;

    /// <summary>
    /// Implements a VFS node which groups <see cref="SiteNode"/> nodes for a single Orchard installation.
    /// </summary>
    public class SitesNode : ContainerNode 
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private readonly TenantAgentProxy tenantAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitesNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="tenantAgent">The tenant agent proxy instance.</param>
        public SitesNode(IOrchardVfs vfs, TenantAgentProxy tenantAgent) 
            : base(vfs, "Sites") 
        {
            this.tenantAgent = tenantAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Sites",
                Description = "Contains all sites (tenants) of the Orchard instance."
            };
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>
        /// A sequence of child nodes.
        /// </returns>
        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() 
        {
            OrchardSite[] sites = this.tenantAgent.GetSites();
            return sites.Select(site => new SiteNode(Vfs, site));
        }
    }
}