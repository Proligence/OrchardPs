// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Nodes 
{
    using System;
    using System.Collections.Generic;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Tenants.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    public class TenantNode : ContainerNode 
    {
        /// <summary>
        /// Cached subnodes of the tenant node.
        /// </summary>
        private List<VfsNode> tenantNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Proligence.PowerShell.Tenants.Nodes.TenantNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="tenant">The <see cref="OrchardTenant"/> object of the tenant represented by the node.</param>
        public TenantNode(IPowerShellVfs vfs, OrchardTenant tenant) 
            : base(vfs, tenant.Name) 
        {
            this.Item = tenant;
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes() 
        {
            if (this.tenantNodes == null) 
            {
                this.tenantNodes = new List<VfsNode>();

                IEnumerable<IPsNavigationProvider> tenantNavigationProviders = 
                    this.Vfs.NavigationProviderManager.GetProviders(NodeType.Site);
                
                foreach (IPsNavigationProvider navigationProvider in tenantNavigationProviders) 
                {
                    if (navigationProvider.Path != "\\") 
                    {
                        throw new NotSupportedException(
                            "Only root paths are supported for tenant navigation providers.");
                    }

                    this.tenantNodes.Add(navigationProvider.Node);
                }
            }

            return this.tenantNodes.ToArray();
        }
    }
}