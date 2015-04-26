namespace Proligence.PowerShell.Provider.Vfs
{
    using System;
    using System.Management.Automation;
    using Orchard;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Provider.Vfs.Nodes;

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
                    var psobj = tenantNode.Item as PSObject;
                    if (psobj != null)
                    {
                        return ((ShellSettings)psobj.ImmediateBaseObject).Name;    
                    }
                    
                    return ((ShellSettings)tenantNode.Item).Name;
                }

                node = node.Parent;
            }

            return null;
        }

        /// <summary>
        /// Executes an action in the context of a tenant.
        /// </summary>
        /// <param name="node">The VFS node which is invoking the action.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        public static void UsingWorkContextScope(
            this VfsNode node,
            string tenantName,
            Action<IWorkContextScope> action)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            node.Vfs.UsingWorkContextScope(tenantName, action);
        }

        /// <summary>
        /// Executes an action in the context of a tenant.
        /// </summary>
        /// <typeparam name="T">The type of the action's result.</typeparam>
        /// <param name="node">The VFS node which is invoking the action.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The action's result.</returns>
        public static T UsingWorkContextScope<T>(
            this VfsNode node,
            string tenantName,
            Func<IWorkContextScope, T> action)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            return node.Vfs.UsingWorkContextScope(tenantName, action);
        }
    }
}