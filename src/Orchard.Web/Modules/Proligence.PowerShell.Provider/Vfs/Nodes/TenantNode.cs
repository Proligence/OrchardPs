namespace Proligence.PowerShell.Provider.Vfs.Nodes 
{
    using System;
    using System.Collections.Generic;
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

        public TenantNode(IPowerShellVfs vfs, ShellSettings tenant) 
            : base(vfs, tenant.Name) 
        {
            this.Item = tenant;
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
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