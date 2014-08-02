namespace Proligence.PowerShell.Provider.Vfs.Provider
{
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Represents the state of a single drive which uses a PS provider based on PowerShell VFS.
    /// </summary>
    public class VfsDriveInfo : PSDriveInfo 
    {
        private readonly IComponentContext container;

        public VfsDriveInfo(PSDriveInfo driveInfo, IComponentContext container)
            : base(driveInfo)
        {
            this.container = container;
            this.NavigationProviderManager = container.Resolve<INavigationProviderManager>();
        }

        /// <summary>
        /// Gets the drive's PowerShell virtual file system (VFS) instance.
        /// </summary>
        public IPowerShellVfs Vfs { get; private set; }

        /// <summary>
        /// Gets the object which exposes the navigation providers for the drive.
        /// </summary>
        public INavigationProviderManager NavigationProviderManager { get; private set; }

        /// <summary>
        /// Initializes the drive.
        /// </summary>
        public virtual void Initialize()
        {
            this.InitializeVfs();
        }

        /// <summary>
        /// Closes the drive and disposes of all used resources.
        /// </summary>
        public virtual void Close()
        {
        }

        /// <summary>
        /// Initializes the drive's PowerShell virtual file system (VFS).
        /// </summary>
        private void InitializeVfs() 
        {
            IPowerShellVfs vfs = new PowerShellVfs(this, this.NavigationProviderManager, new DefaultPathValidator());
            vfs.Initialize();
            this.Vfs = vfs;
        }
    }
}