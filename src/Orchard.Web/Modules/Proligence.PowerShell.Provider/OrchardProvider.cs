namespace Proligence.PowerShell.Provider
{
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Provider;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Implements the Orchard PowerShell Provider.
    /// </summary>
    [CmdletProvider("Orchard", ProviderCapabilities.ShouldProcess)]
    public class OrchardProvider : VfsProvider
    {
        /// <summary>
        /// Gets the object which represents the state of the Orchard PS provider.
        /// </summary>
        public OrchardProviderInfo OrchardProviderInfo
        {
            get
            {
                return (OrchardProviderInfo)this.ProviderInfo;
            }
        }

        /// <summary>
        /// Creates the <see cref="VfsProviderInfo"/> object for the provider.
        /// </summary>
        /// <param name="providerInfo">A <see cref="ProviderInfo"/> object that describes the provider to be initialized.</param>
        /// <returns>The created <see cref="VfsProviderInfo"/> object.</returns>
        protected override VfsProviderInfo CreateProviderInfo(ProviderInfo providerInfo)
        {
            return new OrchardProviderInfo(providerInfo);
        }

        /// <summary>
        /// Initializes the specified VFS drive.
        /// </summary>
        /// <param name="drive">The drive to initialize.</param>
        /// <returns>The <see cref="VfsDriveInfo"/> object which represents the initialized drive.</returns>
        protected override VfsDriveInfo InitializeNewDrive(PSDriveInfo drive)
        {
            var orchardDriveInfo = drive as OrchardDriveInfo;
            if (orchardDriveInfo != null)
            {
                return (VfsDriveInfo)drive;
            }

            VfsDriveInfo orchardDrive = null;
            
            this.TryCritical(
                () => orchardDrive = this.InitializeOrchardDrive(drive),
                ErrorIds.OrchardInitFailed,
                ErrorCategory.OpenError);

            return orchardDrive;
        }

        /// <summary>
        /// Gives the provider the ability to map drives after initialization.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="PSDriveInfo"/> objects for each drive the provider wants to mount. If no drives
        /// should be mounted, an empty collection should be returned.
        /// </returns>
        protected override Collection<PSDriveInfo> InitializeDefaultDrives() 
        {
            var drives = new Collection<PSDriveInfo>();

            this.Try(
                () => 
                {
                    var drive = new PSDriveInfo("Orchard", this.ProviderInfo, "\\", "Orchard drive", this.Credential);
                    drives.Add(this.InitializeOrchardDrive(drive));
                }, 
                ErrorIds.DefaultDrivesInitFailed, 
                ErrorCategory.OpenError);

            return drives;
        }

        /// <summary>
        /// Initializes the specified Orchard drive.
        /// </summary>
        /// <param name="drive">The drive to initialize.</param>
        /// <returns>The <see cref="OrchardDriveInfo"/> object which represents the initialized drive.</returns>
        private OrchardDriveInfo InitializeOrchardDrive(PSDriveInfo drive)
        {
            var privateData = (ConsoleHostPrivateData)this.Host.PrivateData.BaseObject;
            privateData.OrchardDrive = new OrchardDriveInfo(drive, privateData.ComponentContext);
            privateData.OrchardDrive.Initialize();

            return privateData.OrchardDrive;
        }
    }
}