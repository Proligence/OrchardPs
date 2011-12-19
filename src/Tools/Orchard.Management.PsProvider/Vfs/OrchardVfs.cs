using System.Collections.Generic;

namespace Orchard.Management.PsProvider.Vfs {
    internal class OrchardVfs : IOrchardVfs {
        public RootVfsNode Root { get; private set; }
        public INavigationProviderManager NavigationProviderManager { get; private set; }

        public OrchardVfs(OrchardDriveInfo drive, INavigationProviderManager navigationProviderManager) {
            Root = new RootVfsNode(this, drive);
            NavigationProviderManager = navigationProviderManager;
        }

        public void Initialize() {
            IEnumerable<IPsNavigationProvider> globalNodeProviders = NavigationProviderManager.GetProviders(NodeType.Global);
            foreach (IPsNavigationProvider provider in globalNodeProviders) {
                OrchardVfsNode parent = NavigatePath(provider.Path);
                if (parent == null) {
                    throw new OrchardProviderException("Orchard VFS path '" + provider.Path + "' does not exist.");
                }

                ContainerNode containerNode = parent as ContainerNode;
                if (containerNode == null) {
                    throw new OrchardProviderException("The node '" + provider.Path + "' must be a container.");
                }

                containerNode.AddStaticNode(provider.Node);
            }
        }

        public OrchardVfsNode NavigatePath(string path) {
            if (Root != null) {
                return Root.NavigatePath(path);
            }

            return null;
        }
    }
}