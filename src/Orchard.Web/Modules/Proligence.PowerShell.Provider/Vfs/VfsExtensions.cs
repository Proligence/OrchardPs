using System;
using Autofac;
using Orchard;
using Orchard.Data;
using Orchard.Validation;
using Proligence.PowerShell.Provider.Internal;

namespace Proligence.PowerShell.Provider.Vfs {
    public static class VfsExtensions {
        /// <summary>
        /// Executes the specified action in the context of a tenant.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance.</param>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="action">The action to invoke.</param>
        public static void UsingWorkContextScope(
            this IPowerShellVfs vfs,
            string tenantName,
            Action<IWorkContextScope> action) {
            Argument.ThrowIfNull(vfs, "vfs");
            Argument.ThrowIfNull(action, "action");

            // Use explicit transaction if active
            IWorkContextScope explicitScope = GetExplicitTransactionScope(tenantName, vfs);
            if (explicitScope != null) {
                action(explicitScope);
                return;
            }

            var tenantContextManager = vfs.Drive.ComponentContext.Resolve<ITenantContextManager>();

            using (IWorkContextScope scope = tenantContextManager.CreateWorkContextScope(tenantName ?? "Default")) {
                ITransactionManager transactionManager;
                if (!scope.TryResolve(out transactionManager)) {
                    transactionManager = null;
                }

                try {
                    action(scope);
                }
                catch {
                    if (transactionManager != null) {
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
            Func<IWorkContextScope, T> action) {
            Argument.ThrowIfNull(vfs, "vfs");
            Argument.ThrowIfNull(action, "action");

            // Use explicit transaction if active
            IWorkContextScope explicitScope = GetExplicitTransactionScope(tenantName, vfs);
            if (explicitScope != null) {
                return action(explicitScope);
            }

            var tenantContextManager = vfs.Drive.ComponentContext.Resolve<ITenantContextManager>();

            using (IWorkContextScope scope = tenantContextManager.CreateWorkContextScope(tenantName ?? "Default")) {
                ITransactionManager transactionManager;
                if (!scope.TryResolve(out transactionManager)) {
                    transactionManager = null;
                }

                try {
                    return action(scope);
                }
                catch {
                    if (transactionManager != null) {
                        transactionManager.Cancel();
                    }

                    throw;
                }
            }
        }

        private static IWorkContextScope GetExplicitTransactionScope(string tenantName, IPowerShellVfs vfs) {
            var drive = vfs.Drive as OrchardDriveInfo;
            return drive != null
                ? drive.GetTransactionScope(tenantName)
                : null;
        }
    }
}