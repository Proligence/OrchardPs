using System.Collections.Generic;
using System.Linq;

namespace Orchard.Management.PsProvider.Vfs {
    internal class OrchardVfs : IOrchardVfs {
        private readonly RootVfsNode _root;
        public List<IPsNavigationProvider> _navigationProviders;

        public RootVfsNode Root {
            get { return _root; }
        }

        public OrchardVfs(OrchardDriveInfo drive) {
            _root = new RootVfsNode(drive);
        }

        public void Initialize(IEnumerable<IPsNavigationProvider> navigationProviders) {
            IOrderedEnumerable<IPsNavigationProvider> providers = navigationProviders.OrderBy(np => np.GetPathLength());
            _navigationProviders = providers.ToList();

            IEnumerable<IPsNavigationProvider> globalNodeProviders = _navigationProviders
                .Where(np => np.NodeType == NodeType.Global);

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
            if (_root != null) {
                return _root.NavigatePath(path);
            }

            return null;
        }

        public IEnumerable<IPsNavigationProvider> GetNavigationProviders() {
            return new List<IPsNavigationProvider>(_navigationProviders);
        }
    }
}