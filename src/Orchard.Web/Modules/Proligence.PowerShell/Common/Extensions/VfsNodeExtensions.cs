namespace Proligence.PowerShell.Common.Extensions 
{
    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

    /// <summary>
    /// Implements extension methods for the <see cref="VfsNode"/> class.
    /// </summary>
    public static class VfsNodeExtensions 
    {
        /// <summary>
        /// Gets the name of the Orchard tenant which the VFS node belongs to.
        /// </summary>
        /// <param name="node">The orchard VFS node instance.</param>
        /// <returns>
        /// The name of the Orchard tenant which the VFS node belongs to or <c>null</c> if the node does not belong to
        /// any Orchard tenant.
        /// </returns>
        public static string GetCurrentTenantName(this VfsNode node) 
        {
            while (node != null) 
            {
                var tenantNode = node as TenantNode;
                if (tenantNode != null) 
                {
                    return ((ShellSettings)tenantNode.Item).Name;
                }

                node = node.Parent;
            }

            return null;
        }
    }
}