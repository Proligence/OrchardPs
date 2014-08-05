namespace Proligence.PowerShell.Tenants.Nodes 
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    [SupportedCmdlet("Disable-Tenant")]
    [SupportedCmdlet("Remove-Tenant")]
    [SupportedCmdlet("Edit-Tenant")]
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
        /// <param name="tenant">The <see cref="ShellSettings"/> object of the tenant represented by the node.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public TenantNode(IPowerShellVfs vfs, ShellSettings tenant) 
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
                    this.Vfs.NavigationProviderManager.GetProviders(NodeType.Tenant);
                
                foreach (IPsNavigationProvider navigationProvider in tenantNavigationProviders)
                {
                    navigationProvider.Vfs = this.Vfs;
                    navigationProvider.Initialize();

                    if (navigationProvider.Path != "\\") 
                    {
                        throw new NotSupportedException(
                            "Only root paths are supported for tenant navigation providers.");
                    }

                    this.tenantNodes.Add(navigationProvider.Node);
                    navigationProvider.Node.Parent = this;
                }
            }

            return this.tenantNodes.ToArray();
        }
    }
}