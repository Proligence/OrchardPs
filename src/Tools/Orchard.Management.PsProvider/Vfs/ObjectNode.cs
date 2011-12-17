namespace Orchard.Management.PsProvider.Vfs {
    public class ObjectNode : OrchardVfsNode {
        public ObjectNode(string name, object obj) : base(name) {
            Item = obj;
        }
    }
}