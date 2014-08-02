namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using System.Management.Automation.Provider;
    using Proligence.PowerShell.Provider.Console;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Implements the Orchard PS Provider.
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
                return (OrchardProviderInfo)ProviderInfo;
            }
        }

        /// <summary>
        /// Gets the object which can be used to access the PowerShell-controlled output console.
        /// </summary>
        public PsSession Session { get; set; }

        protected ConsoleHost ConsoleHost { get; set; }

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
            if (drive is OrchardDriveInfo)
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
                    var drive = new PSDriveInfo("Orchard", ProviderInfo, "\\", "Orchard drive", Credential);
                    drives.Add(InitializeOrchardDrive(drive));
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
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
        private OrchardDriveInfo InitializeOrchardDrive(PSDriveInfo drive) 
        {
            this.Host.UI.WriteVerboseLine("Initializing Orchard session. (This might take a few seconds...)");
            this.Host.UI.WriteLine();
            
            var orchardDrive = new OrchardDriveInfo(drive);
            orchardDrive.Initialize();

            this.Host.UI.WriteLine(ConsoleColor.Yellow, 0, "                    Welcome to Orchard PowerShell!");

            this.Host.UI.WriteLine();
            this.Host.UI.WriteLine(
                "To get a list of all Orchard-related cmdlets, type Get-OrchardPsCommand -All.");
            this.Host.UI.WriteLine(
                "To get a list of all supported cmdlets for the current location type Get-OrchardPsCommand.");
            this.Host.UI.WriteLine(
                "To get help about a specific cmdlet, type Get-Help CommandName.");
            this.Host.UI.WriteLine(
                "To get more help about the Orchard PowerShell provider, type Get-Help Orchard.");
            this.Host.UI.WriteLine();

            return orchardDrive;
        }
    }
}