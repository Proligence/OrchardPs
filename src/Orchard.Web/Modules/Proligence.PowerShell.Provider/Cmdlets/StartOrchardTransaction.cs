namespace Proligence.PowerShell.Provider.Cmdlets
{
    using System.Management.Automation;
    using Autofac;
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Internal;
    using Proligence.PowerShell.Provider.Utilities;

    [Cmdlet(VerbsLifecycle.Start, "OrchardTransaction", DefaultParameterSetName = "Default", SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.None)]
    public class StartOrchardTransaction : OrchardTransactionCmdletBase
    {
        protected override void ProcessRecord(ShellSettings tenant, OrchardDriveInfo drive)
        {
            if (this.EnsureTransactionNotActive(tenant.Name, drive))
            {
                var tenantContextManager = drive.ComponentContext.Resolve<ITenantContextManager>();
                var scope = tenantContextManager.CreateWorkContextScope(tenant.Name);
                drive.SetTransactionScope(tenant.Name, scope);
            }
        }

        protected bool EnsureTransactionNotActive(string tenantName, OrchardDriveInfo drive)
        {
            if (drive.GetTransactionScope(tenantName) != null)
            {
                this.WriteError(Error.InvalidOperation(
                    "An Orchard transaction is already active. Please complete or undo it before starting a new transaction.",
                    "ActiveOrchardTransaction"));

                return false;
            }

            return true;
        }
    }
}