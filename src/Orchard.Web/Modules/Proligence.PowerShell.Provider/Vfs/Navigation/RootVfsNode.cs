using Proligence.PowerShell.Provider.Vfs.Provider;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Represents the root (drive) node in the PowerShell VFS.
    /// </summary>
    public class RootVfsNode : ContainerNode {
        public RootVfsNode(IPowerShellVfs vfs, VfsDriveInfo drive)
            : base(vfs, null) {
            Item = drive;
        }
    }
}