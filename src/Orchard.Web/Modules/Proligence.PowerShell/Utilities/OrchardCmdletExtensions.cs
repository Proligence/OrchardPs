namespace Proligence.PowerShell.Utilities
{
    using System;
    using Autofac;
    using Orchard;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
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

        /// <summary>
        /// Executes an action in the context of a tenant.
        /// </summary>
        /// <param name="cmdlet">The cmdlet which is invoking the action.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        public static void UsingWorkContextScope(
            this OrchardCmdlet cmdlet,
            string tenantName,
            Action<IWorkContextScope> action)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            cmdlet.CurrentNode.Vfs.UsingWorkContextScope(tenantName, action);
        }

        /// <summary>
        /// Executes an action in the context of a tenant.
        /// </summary>
        /// <typeparam name="T">The type of the action's result.</typeparam>
        /// <param name="cmdlet">The VFS node which is invoking the action.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The action's result.</returns>
        public static T UsingWorkContextScope<T>(
            this OrchardCmdlet cmdlet,
            string tenantName,
            Func<IWorkContextScope, T> action)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            return cmdlet.CurrentNode.Vfs.UsingWorkContextScope(tenantName, action);
        }

        /// <summary>
        /// Resolves the specified service from the global scope.
        /// </summary>
        /// <typeparam name="T">The type of service to resolve.</typeparam>
        /// <returns>The resolved service instance.</returns>
        public static T Resolve<T>(this OrchardCmdlet cmdlet)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }
            
            return cmdlet.CurrentNode.Vfs.Drive.ComponentContext.Resolve<T>();
        }
    }
}