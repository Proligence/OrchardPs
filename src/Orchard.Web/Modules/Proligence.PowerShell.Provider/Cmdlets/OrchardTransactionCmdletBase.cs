namespace Proligence.PowerShell.Provider.Cmdlets
{
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Internal;
    using Proligence.PowerShell.Provider.Utilities;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public abstract class OrchardTransactionCmdletBase : TenantCmdlet
    {
        protected abstract void ProcessRecord(ShellSettings tenant, OrchardDriveInfo drive);

        protected override void ProcessRecord(ShellSettings tenant)
        {
            VfsNode currentNode = this.CurrentNode;
            if (currentNode != null)
            {
                var drive = currentNode.Vfs.Drive as OrchardDriveInfo;
                if (drive != null)
                {
                    this.ProcessRecord(tenant, drive);
                    return;
                }
            }

            this.WriteOrchardDriveExpectedError();
        }

        private void WriteOrchardDriveExpectedError()
        {
            this.WriteError(Error.InvalidOperation(
                "The cmdlet must be called on an Orchard drive.",
                "OrchardDriveExpected"));
        }

        protected bool EnsureTransactionActive(string tenantName, OrchardDriveInfo drive)
        {
            if (drive.GetTransactionScope(tenantName) == null)
            {
                this.WriteError(Error.InvalidOperation(
                    "There is no active Orchard transaction.",
                    "NoActiveTransaction"));

                return false;
            }

            return true;
        }
    }
}