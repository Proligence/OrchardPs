namespace Proligence.PowerShell.Provider.Vfs
{
    using System;
    using Autofac;
    using Orchard;
    using Orchard.Data;
    using Orchard.Validation;

    public static class VfsExtensions
    {
        /// <summary>
        /// Executes the specified action in the context of a tenant.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        public static void UsingWorkContextScope(
            this IPowerShellVfs vfs,
            string tenantName,
            Action<IWorkContextScope> action)
        {
            Argument.ThrowIfNull(vfs, "vfs");
            Argument.ThrowIfNull(action, "action");
            
            var tenantContextManager = vfs.Drive.ComponentContext.Resolve<ITenantContextManager>();

            using (IWorkContextScope scope = tenantContextManager.CreateWorkContextScope(tenantName ?? "Default"))
            {
                ITransactionManager transactionManager;
                if (!scope.TryResolve(out transactionManager))
                {
                    transactionManager = null;
                }

                try
                {
                    action(scope);
                }
                catch
                {
                    if (transactionManager != null)
                    {
                        transactionManager.Cancel();
                    }

                    throw;
                }
            }
        }

        /// <summary>
        /// Executes the specified action in the context of a tenant.
        /// </summary>
        /// <typeparam name="T">The type of the action's result.</typeparam>
        /// <param name="vfs">The Orchard VFS instance.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The result returned by the action.</returns>
        public static T UsingWorkContextScope<T>(
            this IPowerShellVfs vfs,
            string tenantName,
            Func<IWorkContextScope, T> action)
        {
            Argument.ThrowIfNull(vfs, "vfs");
            Argument.ThrowIfNull(action, "action");

            var tenantContextManager = vfs.Drive.ComponentContext.Resolve<ITenantContextManager>();

            using (IWorkContextScope scope = tenantContextManager.CreateWorkContextScope(tenantName ?? "Default"))
            {
                ITransactionManager transactionManager;
                if (!scope.TryResolve(out transactionManager))
                {
                    transactionManager = null;
                }

                try
                {
                    return action(scope);
                }
                catch
                {
                    if (transactionManager != null)
                    {
                        transactionManager.Cancel();
                    }

                    throw;
                }
            }
        }
    }
}