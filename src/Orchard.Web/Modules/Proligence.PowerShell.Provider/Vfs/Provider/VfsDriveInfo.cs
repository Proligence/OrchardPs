namespace Proligence.PowerShell.Provider.Vfs.Provider
{
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Represents the state of a single drive which uses a PS provider based on PowerShell VFS.
    /// </summary>
    public class VfsDriveInfo : PSDriveInfo 
    {
        public VfsDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo)
        {
            this.ComponentContext = componentContext;
        }

        /// <summary>
        /// Gets the dependency injection container of the Orchard application.
        /// </summary>
        public IComponentContext ComponentContext { get; private set; }

        /// <summary>
        /// Gets the drive's PowerShell virtual file system (VFS) instance.
        /// </summary>
        public IPowerShellVfs Vfs { get; private set; }

        /// <summary>
        /// Initializes the drive.
        /// </summary>
        public virtual void Initialize()
        {
            var navigationProviderManager = this.ComponentContext.Resolve<INavigationProviderManager>();
            var pathValidator = this.ComponentContext.Resolve<IPathValidator>();
            this.Vfs = new PowerShellVfs(this, navigationProviderManager, pathValidator);
            this.Vfs.Initialize();
        }

        /// <summary>
        /// Closes the drive and disposes of all used resources.
        /// </summary>
        public virtual void Close()
        {
        }
    }
}