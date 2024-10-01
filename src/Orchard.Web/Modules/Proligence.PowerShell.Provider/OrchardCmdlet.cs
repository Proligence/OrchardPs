using System;
using System.Management.Automation;
using Proligence.PowerShell.Provider.Internal;
using Proligence.PowerShell.Provider.Utilities;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// The base class for cmdlets which implement Orchard-related management functionality.
    /// </summary>
    /// <remarks>
    /// This class adds the <see cref="OrchardDrive"/> and <see cref="CurrentNode"/> properties which are initialized
    /// based on the current location in the Orchard VFS. If the cmdlet is invoked from a path which belongs to the
    /// Orchard PowerShell provider, then the underlying drive and VFS not is used to initialize these properties.
    /// Otherwise, the default 'Orchard' drive is used as a fallback.
    /// </remarks>
    public class OrchardCmdlet : PSCmdlet, IOrchardCmdlet {
        /// <summary>
        /// Gets the <see cref="OrchardDriveInfo"/> object of the Orchard drive on which the cmdlet was called.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; private set; }

        /// <summary>
        /// Gets the current VFS node in the Orchard drive on which the cmdlet was called.
        /// </summary>
        public VfsNode CurrentNode { get; private set; }

        protected override void BeginProcessing() {
            OrchardDrive = SessionState.Drive.Current as OrchardDriveInfo;
            if (OrchardDrive == null) {
                OrchardDrive = SessionState.Drive.Get("Orchard") as OrchardDriveInfo;
                if (OrchardDrive == null) {
                    var exception = new InvalidOperationException(
                        "Failed to find Orchard drive. Make sure that the Orchard VFS is properly initialized and " +
                        "the Orchard PSDrive is mounted.");

                    ThrowTerminatingError(Error.InvalidOperation(exception, ErrorIds.OrchardDriveExpected));
                }
            }

            // ReSharper disable once PossibleNullReferenceException
            CurrentNode = OrchardDrive.Vfs.NavigatePath(OrchardDrive.CurrentLocation);
        }
    }
}