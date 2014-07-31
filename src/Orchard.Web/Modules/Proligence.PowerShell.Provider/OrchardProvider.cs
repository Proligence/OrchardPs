namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Management.Automation.Provider;
    using System.Reflection;
    using Autofac;
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
        /// Gets the dependency injection container for the provider.
        /// </summary>
        /// <returns>The <see cref="IContainer"/> instance.</returns>
        protected override IContainer GetContainer()
        {
            return null;
        }

        /// <summary>
        /// Creates the <see cref="VfsProviderInfo"/> object for the provider.
        /// </summary>
        /// <param name="providerInfo">A <see cref="ProviderInfo"/> object that describes the provider to be initialized.</param>
        /// <param name="container">The dependency injection container.</param>
        /// <returns>The created <see cref="VfsProviderInfo"/> object.</returns>
        protected override VfsProviderInfo CreateProviderInfo(ProviderInfo providerInfo, IContainer container)
        {
            return new OrchardProviderInfo(providerInfo, container);
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
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    string path = entryAssembly != null ? entryAssembly.Location : Environment.CurrentDirectory;

                    for (var di = new DirectoryInfo(path); di != null; di = di.Parent) 
                    {
                        /*
                        if (OrchardPsSnapIn.VerifyOrchardDirectory(di.FullName)) 
                        {
                            var drive = new PSDriveInfo("Orchard", ProviderInfo, "\\", "Orchard drive", Credential);
                            var driveParameters = new OrchardDriveParameters { OrchardRoot = di.FullName };
                            drives.Add(InitializeOrchardDrive(drive, driveParameters));
                            break;
                        }*/
                    }
                }, 
                ErrorIds.DefaultDrivesInitFailed, 
                ErrorCategory.OpenError);

            return drives;
        }

        /// <summary>
        /// Increases the size of the console window of the host process.
        /// </summary>
        /// <param name="width">The new width of the window.</param>
        /// <param name="height">The new height of the window.</param>
        private void IncreaseWindowSize(int width, int height) 
        {
            Size maximumSize = Host.UI.RawUI.MaxPhysicalWindowSize;
            Size bufferSize = Host.UI.RawUI.BufferSize;
            Size windowSize = Host.UI.RawUI.WindowSize;

            bool updateNeeded = false;

            if (bufferSize.Width < width) 
            {
                bufferSize.Width = (width <= maximumSize.Width) ? width : maximumSize.Width;
                updateNeeded = true;
            }

            if (windowSize.Width < width) 
            {
                windowSize.Width = (width <= maximumSize.Width) ? width : maximumSize.Width;
                updateNeeded = true;
            }

            if (bufferSize.Height < height) 
            {
                bufferSize.Height = (height < maximumSize.Height) ? height : maximumSize.Height;
                updateNeeded = true;
            }

            if (windowSize.Height < height) 
            {
                windowSize.Height = (height <= maximumSize.Height) ? height : maximumSize.Height;
                updateNeeded = true;
            }

            if (updateNeeded) 
            {
                Host.UI.RawUI.BufferSize = bufferSize;
                Host.UI.RawUI.WindowSize = windowSize;
            }
        }

        /// <summary>
        /// Initializes the specified Orchard drive.
        /// </summary>
        /// <param name="drive">The drive to initialize.</param>
        /// <returns>The <see cref="OrchardDriveInfo"/> object which represents the initialized drive.</returns>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly")]
        private OrchardDriveInfo InitializeOrchardDrive(PSDriveInfo drive) 
        {
            this.Console.WriteVerbose("Initializing Orchard session. (This might take a few seconds...)");
            this.Console.WriteLine();
            
            var orchardDrive = new OrchardDriveInfo(drive);
            orchardDrive.Initialize();

            var color = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.White;
            System.Console.WriteLine("                    Welcome to Orchard PowerShell!");
            System.Console.ForegroundColor = color;

            System.Console.WriteLine();
            System.Console.WriteLine(
                "To get a list of all Orchard-related cmdlets, type Get-OrchardPsCommand -All.");
            System.Console.WriteLine(
                "To get a list of all supported cmdlets for the current location type Get-OrchardPsCommand.");
            System.Console.WriteLine(
                "To get help about a specific cmdlet, type Get-Help CommandName.");
            System.Console.WriteLine(
                "To get more help about the Orchard PowerShell provider, type Get-Help Orchard.");
            System.Console.WriteLine();

            return orchardDrive;
        }
    }
}