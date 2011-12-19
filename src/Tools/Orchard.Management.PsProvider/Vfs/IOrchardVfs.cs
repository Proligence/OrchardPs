namespace Orchard.Management.PsProvider.Vfs {
    public interface IOrchardVfs {
        RootVfsNode Root { get; }
        INavigationProviderManager NavigationProviderManager { get; }
        void Initialize();
        OrchardVfsNode NavigatePath(string path);
    }
}