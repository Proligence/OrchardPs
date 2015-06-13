namespace Proligence.PowerShell.Provider.Internal
{
    using System.Collections.Generic;
    using System.Management.Automation;
    using Autofac;
    using Orchard;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a PowerShell drive which is handled by the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : VfsDriveInfo
    {
        private readonly IDictionary<string, IWorkContextScope> transactionScopes = new Dictionary<string, IWorkContextScope>();

        public OrchardDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo, componentContext)
        {
        }

        /// <summary>
        /// Sets the work context scope used for the current explict transaction.
        /// </summary>
        /// <param name="tenantName">The name of the tenant for which the expicit transaction scope will be set.</param>
        /// <param name="scope">The work context scope instance.</param>
        public void SetTransactionScope(string tenantName, IWorkContextScope scope)
        {
            if (scope != null)
            {
                this.transactionScopes.Add(tenantName, scope);                
            }
            else
            {
                this.transactionScopes.Remove(tenantName);
            }
        }

        /// <summary>
        /// Gets the work context scope used for the current explict transaction. Returns null if there is no explicit
        /// transaction for the specified tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant for which the expicit transaction scope will be retrieved.</param>
        public IWorkContextScope GetTransactionScope(string tenantName)
        {
            IWorkContextScope scope;
            if (this.transactionScopes.TryGetValue(tenantName, out scope))
            {
                return scope;
            }

            return null;
        }
    }
}