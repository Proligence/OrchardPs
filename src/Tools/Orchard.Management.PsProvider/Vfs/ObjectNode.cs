namespace Orchard.Management.PsProvider.Vfs {
    public class ObjectNode : OrchardVfsNode {
        public ObjectNode(IOrchardVfs vfs, string name, object obj) : base(vfs, name) {
            Item = obj;
        }
    }
}