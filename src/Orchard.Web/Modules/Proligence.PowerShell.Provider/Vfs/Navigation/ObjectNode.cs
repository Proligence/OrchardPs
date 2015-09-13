namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// The base class for VFS nodes which encapsulate item objects.
    /// </summary>
    public class ObjectNode : VfsNode {
        public ObjectNode(IPowerShellVfs vfs, string name, object obj)
            : base(vfs, name) {
            Item = obj;
        }
    }
}