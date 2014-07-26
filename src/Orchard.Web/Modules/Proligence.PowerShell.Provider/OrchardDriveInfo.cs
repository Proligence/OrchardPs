namespace Proligence.PowerShell.Provider
{
    using System.IO;
    using System.Management.Automation;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a single Orchard drive in the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : VfsDriveInfo 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardDriveInfo"/> class.
        /// </summary>
        /// <param name="driveInfo">The <see cref="DriveInfo"/> object for the Orchard drive.</param>
        public OrchardDriveInfo(PSDriveInfo driveInfo)
            : base(driveInfo, null)
        {
        }

        /// <summary>
        /// Initializes the drive and establishes a connection with the AppDomain which runs inside the Orchard web
        /// application.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Closes the drive and shutdowns the connection with the AppDomain which runs inside the Orchard web
        /// application.
        /// </summary>
        public override void Close()
        {
            base.Close();
        }
    }
}