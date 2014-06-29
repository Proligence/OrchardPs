// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantsNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Tenants.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="TenantNode"/> nodes for a single Orchard installation.
    /// </summary>
    [SupportedCmdlet("Enable-Tenant")]
    [SupportedCmdlet("Disable-Tenant")]
    [SupportedCmdlet("Remove-Tenant")]
    [SupportedCmdlet("Edit-Tenant")]
    public class TenantsNode : ContainerNode 
    {
        /// <summary>
        /// The tenant agent instance.
        /// </summary>
        private readonly ITenantAgent tenantAgent;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantsNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="tenantAgent">The tenant agent instance.</param>
        public TenantsNode(IPowerShellVfs vfs, ITenantAgent tenantAgent) 
            : base(vfs, "Tenants") 
        {
            this.tenantAgent = tenantAgent;

            this.Item = new CollectionItem(this) 
            {
                Name = "Tenants",
                Description = "Contains all tenants of the Orchard instance."
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
            OrchardTenant[] tenants = this.tenantAgent.GetTenants();
            return tenants.Select(tenant => new TenantNode(Vfs, tenant));
        }
    }
}