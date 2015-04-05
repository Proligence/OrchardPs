namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Management.Automation;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// The base class for cmdlets which implement Orchard-related management functionality.
    /// </summary>
    /// <remarks>
    /// This class adds the <see cref="OrchardDrive"/> and <see cref="CurrentNode"/> properties which are initlized
    /// based on the current location in the Orchard VFS. If the cmdlet is invoked from a path which belongs to the
    /// Orchard PowerShell provider, then the underlying drive and VFS not is used to initialize these properties.
    /// Otherwise, the default 'Orchard' drive is used as a fallback.
    /// </remarks>
    public class OrchardCmdlet : PSCmdlet, IOrchardCmdlet
    {
        /// <summary>
        /// Gets the <see cref="OrchardDriveInfo"/> object of the Orchard drive on which the cmdlet was called.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; private set; }

        /// <summary>
        /// Gets the current VFS node in the Orchard drive on which the cmdlet was called.
        /// </summary>
        public VfsNode CurrentNode { get; private set; }

        protected override void BeginProcessing() 
        {
            this.OrchardDrive = this.SessionState.Drive.Current as OrchardDriveInfo;
            if (this.OrchardDrive == null)
            {
                this.OrchardDrive = this.SessionState.Drive.Get("Orchard") as OrchardDriveInfo;
                if (this.OrchardDrive == null)
                {
                    var exception = new InvalidOperationException(
                        "Failed to find Orchard drive. Make sure that the Orchard VFS is properly initialized and " +
                        "the Orchard PSDrive is mounted.");

                    this.ThrowTerminatingError(exception, ErrorIds.OrchardDriveExpected, ErrorCategory.InvalidOperation);
                }
            }

            this.CurrentNode = this.OrchardDrive.Vfs.NavigatePath(this.OrchardDrive.CurrentLocation);
        }
    }
}