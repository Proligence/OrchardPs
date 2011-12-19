using System.Collections.Generic;

namespace Orchard.Management.PsProvider.Vfs {
    public interface INavigationProviderManager {
        IEnumerable<IPsNavigationProvider> GetProviders();
        IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType);
    }
}