// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Sites.Nodes 
{
    using System;
    using System.Collections.Generic;
    using Proligence.PowerShell.Sites.Items;
    using Proligence.PowerShell.Vfs;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard site.
    /// </summary>
    public class SiteNode : ContainerNode 
    {
        /// <summary>
        /// Cached subnodes of the site node.
        /// </summary>
        private List<VfsNode> siteNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="SiteNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="site">The <see cref="OrchardSite"/> object of the site represented by the node.</param>
        public SiteNode(IPowerShellVfs vfs, OrchardSite site) 
            : base(vfs, site.Name) 
        {
            this.Item = site;
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes() 
        {
            if (this.siteNodes == null) 
            {
                this.siteNodes = new List<VfsNode>();

                IEnumerable<IPsNavigationProvider> siteNavigationProviders = 
                    this.Vfs.NavigationProviderManager.GetProviders(NodeType.Site);
                
                foreach (IPsNavigationProvider navigationProvider in siteNavigationProviders) 
                {
                    if (navigationProvider.Path != "\\") 
                    {
                        throw new NotSupportedException(
                            "Only root paths are supported for site navigation providers.");
                    }

                    this.siteNodes.Add(navigationProvider.Node);
                }
            }

            return this.siteNodes.ToArray();
        }
    }
}