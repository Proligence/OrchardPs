using System.Management.Automation;
using Autofac;
using Orchard;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Vfs.Provider {
    /// <summary>
    /// Represents the state of a single drive which uses a PS provider based on PowerShell VFS.
    /// </summary>
    public class VfsDriveInfo : PSDriveInfo {
        public VfsDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo) {
            ComponentContext = componentContext;
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
        public virtual void Initialize() {
            using (var wca = ComponentContext.Resolve<IWorkContextAccessor>().CreateWorkContextScope()) {
                var navigationProviderManager = wca.Resolve<INavigationProviderManager>();
                var pathValidator = ComponentContext.Resolve<IPathValidator>();
                Vfs = new PowerShellVfs(this, navigationProviderManager, pathValidator);
                Vfs.Initialize();
            }
        }

        /// <summary>
        /// Closes the drive and disposes of all used resources.
        /// </summary>
        public virtual void Close() {
        }
    }
}