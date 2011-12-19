namespace Orchard.Management.PsProvider.Vfs {
    public class RootVfsNode : ContainerNode {
        public RootVfsNode(IOrchardVfs vfs, OrchardDriveInfo drive) : base(vfs, null) {
            Item = drive;
        }
    }
}