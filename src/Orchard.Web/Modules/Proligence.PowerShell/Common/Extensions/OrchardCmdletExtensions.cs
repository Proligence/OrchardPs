namespace Proligence.PowerShell.Common.Extensions 
{
    using System;

    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

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
        /// The <see cref="ShellSettings"/> object which represents the current tenant or <c>null</c> if the cmdlet was
        /// invoked from a path which is not under any Orchard tenant.
        /// </returns>
        public static ShellSettings GetCurrentTenant(this OrchardCmdlet cmdlet) 
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
                    return tenantNode.Item as ShellSettings;
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

            ShellSettings tenant = cmdlet.GetCurrentTenant();
            if (tenant != null)
            {
                return tenant.Name;
            }

            return null;
        }
    }
}