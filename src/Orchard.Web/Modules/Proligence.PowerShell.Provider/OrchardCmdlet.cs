namespace Proligence.PowerShell.Provider
{
    using System.Management.Automation;
    using Proligence.PowerShell.Provider.Internal;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// The base class for cmdlets which must be invoked from a path which belongs to the Orchard PS provider.
    /// </summary>
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

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing() 
        {
            this.OrchardDrive = SessionState.Drive.Current as OrchardDriveInfo;
            if (this.OrchardDrive == null) 
            {
                var exception = ThrowHelper.InvalidOperation("The cmdlet must be invoked from an Orchard drive.");
                this.ThrowTerminatingError(exception, ErrorIds.OrchardDriveExpected, ErrorCategory.InvalidOperation);
            }

            this.CurrentNode = this.OrchardDrive.Vfs.NavigatePath(this.OrchardDrive.CurrentLocation);
        }
    }
}