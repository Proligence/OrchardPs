using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Internal;
using Proligence.PowerShell.Provider.Utilities;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Cmdlets {
    public abstract class OrchardTransactionCmdletBase : TenantCmdlet {
        protected abstract void ProcessRecord(ShellSettings tenant, OrchardDriveInfo drive);

        protected override void ProcessRecord(ShellSettings tenant) {
            VfsNode currentNode = CurrentNode;
            if (currentNode != null) {
                var drive = currentNode.Vfs.Drive as OrchardDriveInfo;
                if (drive != null) {
                    ProcessRecord(tenant, drive);
                    return;
                }
            }

            WriteOrchardDriveExpectedError();
        }

        private void WriteOrchardDriveExpectedError() {
            WriteError(Error.InvalidOperation(
                "The cmdlet must be called on an Orchard drive.",
                "OrchardDriveExpected"));
        }

        protected bool EnsureTransactionActive(string tenantName, OrchardDriveInfo drive) {
            if (drive.GetTransactionScope(tenantName) == null) {
                WriteError(Error.InvalidOperation(
                    "There is no active Orchard transaction.",
                    "NoActiveTransaction"));

                return false;
            }

            return true;
        }
    }
}