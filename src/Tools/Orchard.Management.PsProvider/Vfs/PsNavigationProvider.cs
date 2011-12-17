using Orchard.Management.PsProvider.Agents;

namespace Orchard.Management.PsProvider.Vfs {
    public abstract class PsNavigationProvider : IPsNavigationProvider {
        protected PsNavigationProvider() { }
        protected PsNavigationProvider(NodeType nodeType, string path, OrchardVfsNode node) {
            Path = path;
            NodeType = nodeType;
            Node = node;
        }

        public string Path { get; protected set; }
        public NodeType NodeType { get; protected set; }
        public OrchardVfsNode Node { get; protected set; }
        public IAgentManager AgentManager { get; set; }

        public virtual void Initialize() { }
    }
}