﻿using System;
using System.Collections.Generic;
using System.Management.Automation;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Vfs.Nodes {
    /// <summary>
    /// Implements a VFS node which represents an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    [SupportedCmdlet("Disable-Tenant")]
    [SupportedCmdlet("Remove-Tenant")]
    [SupportedCmdlet("Edit-Tenant")]
    public class TenantNode : ContainerNode {
        /// <summary>
        /// Cached subnodes of the tenant node.
        /// </summary>
        private List<VfsNode> _tenantNodes;

        public TenantNode(IPowerShellVfs vfs, ShellSettings tenant)
            : base(vfs, tenant.Name) {
            Item = tenant;
        }

        public TenantNode(IPowerShellVfs vfs, PSObject tenant, string name)
            : base(vfs, name) {
            Item = tenant;
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            if (_tenantNodes == null) {
                _tenantNodes = new List<VfsNode>();

                IEnumerable<IPsNavigationProvider> tenantNavigationProviders =
                    Vfs.NavigationProviderManager.GetProviders(NodeType.Tenant);

                foreach (IPsNavigationProvider navigationProvider in tenantNavigationProviders) {
                    navigationProvider.Vfs = Vfs;
                    navigationProvider.Initialize();

                    if (navigationProvider.Path != "\\") {
                        throw new NotSupportedException(
                            "Only root paths are supported for tenant navigation providers.");
                    }

                    _tenantNodes.Add(navigationProvider.Node);
                    navigationProvider.Node.Parent = this;
                }
            }

            return _tenantNodes.ToArray();
        }
    }
}