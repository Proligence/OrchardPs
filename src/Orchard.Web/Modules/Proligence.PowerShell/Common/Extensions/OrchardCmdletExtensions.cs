namespace Proligence.PowerShell.Common.Extensions 
{
    using System;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Tenants.Items;
    using Proligence.PowerShell.Tenants.Nodes;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements extension methods for the <see cref="OrchardCmdlet"/> class.
    /// </summary>
    public static class OrchardCmdletExtensions 
    {
        /// <summary>
        /// Gets the Orchard tenant from which the cmdlet was invoked.
        /// </summary>
        /// <param name="cmdlet">The cmdlet instance.</param>
        /// <returns>
        /// The <see cref="OrchardTenant"/> object which represents the current tenant or <c>null</c> if the cmdlet was
        /// invoked from a path which is not under any Orchard tenant.
        /// </returns>
        public static OrchardTenant GetCurrentTenant(this OrchardCmdlet cmdlet) 
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            VfsNode currentNode = cmdlet.CurrentNode;
            while (currentNode != null) 
            {
                var tenantNode = currentNode as TenantNode;
                if (tenantNode != null) 
                {
                    return tenantNode.Item as OrchardTenant;
                }
                
                currentNode = currentNode.Parent;
            }

            return null;
        }

        /// <summary>
        /// Gets the name of the Orchard tenant from which the cmdlet was invoked.
        /// </summary>
        /// <param name="cmdlet">The cmdlet instance.</param>
        /// <returns>
        /// The name of the current tenant or <c>null</c> if the cmdlet was invoked from a path which is not under any
        /// Orchard tenant.
        /// </returns>
        public static string GetCurrentTenantName(this OrchardCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            OrchardTenant tenant = cmdlet.GetCurrentTenant();
            if (tenant != null)
            {
                return tenant.Name;
            }

            return null;
        }
    }
}