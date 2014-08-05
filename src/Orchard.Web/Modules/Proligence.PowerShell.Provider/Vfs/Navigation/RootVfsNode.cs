namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the root (drive) node in the PowerShell VFS.
    /// </summary>
    public class RootVfsNode : ContainerNode 
    {
        public RootVfsNode(IPowerShellVfs vfs, VfsDriveInfo drive) 
            : base(vfs, null) 
        {
            this.Item = drive;
        }
    }
}