// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitesNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Sites.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="SiteNode"/> nodes for a single Orchard installation.
    /// </summary>
    public class SitesNode : ContainerNode 
    {
        /// <summary>
        /// The tenant agent instance.
        /// </summary>
        private readonly ITenantAgent tenantAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="SitesNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="tenantAgent">The tenant agent instance.</param>
        public SitesNode(IPowerShellVfs vfs, ITenantAgent tenantAgent) 
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
        public override IEnumerable<VfsNode> GetVirtualNodes() 
        {
            OrchardSite[] sites = this.tenantAgent.GetSites();
            return sites.Select(site => new SiteNode(Vfs, site));
        }
    }
}