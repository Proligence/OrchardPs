namespace Orchard.Management.PsProvider.Vfs {
    public interface IPsNavigationProvider {
        string Path { get; }
        NodeType NodeType { get; }
        OrchardVfsNode Node { get; }
        
        void Initialize();
    }
}