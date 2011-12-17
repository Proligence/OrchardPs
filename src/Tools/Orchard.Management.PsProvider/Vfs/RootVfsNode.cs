namespace Orchard.Management.PsProvider.Vfs {
    public class RootVfsNode : ContainerNode
    {
        public RootVfsNode(OrchardDriveInfo drive) : base(null) {
            Item = drive;
        }
    }
}