using System.Collections.Generic;
using System.Management.Automation;
using Autofac;
using Orchard;
using Proligence.PowerShell.Provider.Vfs.Provider;

namespace Proligence.PowerShell.Provider.Internal {
    /// <summary>
    /// Represents the state of a PowerShell drive which is handled by the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : VfsDriveInfo {
        private readonly IDictionary<string, IWorkContextScope> _transactionScopes =
            new Dictionary<string, IWorkContextScope>();

        public OrchardDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo, componentContext) {
        }

        internal IDictionary<string, IWorkContextScope> TransactionScopes {
            get { return _transactionScopes; }
        }

        /// <summary>
        /// Sets the work context scope used for the current explicit transaction.
        /// </summary>
        /// <param name="tenantName">The name of the tenant for which the explicit transaction scope will be set.</param>
        /// <param name="scope">The work context scope instance.</param>
        public void SetTransactionScope(string tenantName, IWorkContextScope scope) {
            if (scope != null) {
                _transactionScopes.Add(tenantName, scope);
            }
            else {
                _transactionScopes.Remove(tenantName);
            }
        }

        /// <summary>
        /// Gets the work context scope used for the current explicit transaction. Returns null if there is no explicit
        /// transaction for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant for which the explicit transaction scope will be retrieved.</param>
        public IWorkContextScope GetTransactionScope(string tenantName) {
            IWorkContextScope scope;
            if (_transactionScopes.TryGetValue(tenantName, out scope)) {
                return scope;
            }

            return null;
        }
    }
}