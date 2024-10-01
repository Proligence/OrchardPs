using System.Management.Automation;
using Orchard;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Internal;

namespace Proligence.PowerShell.Provider.Cmdlets {
    [Cmdlet(VerbsLifecycle.Complete, "OrchardTransaction", DefaultParameterSetName = "Default", SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.None)]
    public class CompleteOrchardTransaction : OrchardTransactionCmdletBase {
        protected override void ProcessRecord(ShellSettings tenant, OrchardDriveInfo drive) {
            if (EnsureTransactionActive(tenant.Name, drive)) {
                IWorkContextScope scope = drive.GetTransactionScope(tenant.Name);
                drive.SetTransactionScope(tenant.Name, null);
                scope.Dispose();
            }
        }
    }
}