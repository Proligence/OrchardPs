using System.Management.Automation;
using Orchard;
using Orchard.Data;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Internal;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Provider.Cmdlets {
    [Cmdlet(VerbsCommon.Undo, "OrchardTransaction", DefaultParameterSetName = "Default", SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.None)]
    public class UndoOrchardTransaction : OrchardTransactionCmdletBase {
        protected override void ProcessRecord(ShellSettings tenant, OrchardDriveInfo drive) {
            if (EnsureTransactionActive(tenant.Name, drive)) {
                IWorkContextScope scope = drive.GetTransactionScope(tenant.Name);
                drive.SetTransactionScope(tenant.Name, null);

                ITransactionManager transactionManager;
                if (scope.TryResolve(out transactionManager)) {
                    transactionManager.Cancel();
                }
                else {
                    WriteError(Error.InvalidOperation("Failed to resolve transaction manager.", "FailedToResolveTM"));
                }

                scope.Dispose();
            }
        }
    }
}