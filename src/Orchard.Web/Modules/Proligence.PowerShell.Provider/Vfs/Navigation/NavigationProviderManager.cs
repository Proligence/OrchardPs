using System.Collections.Generic;
using System.Linq;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    public class NavigationProviderManager : INavigationProviderManager {
        private readonly IPsNavigationProvider[] _navigationProviders;

        public NavigationProviderManager(IEnumerable<IPsNavigationProvider> navigationProviders) {
            _navigationProviders = navigationProviders.ToArray();
        }

        /// <summary>
        /// Gets all registered PS navigation providers.
        /// </summary>
        /// <returns>A sequence of navigation providers.</returns>
        public IEnumerable<IPsNavigationProvider> GetProviders() {
            return (IEnumerable<IPsNavigationProvider>) _navigationProviders.Clone();
        }

        /// <summary>
        /// Gets all registered PS navigation providers which add a node of the specified type.
        /// </summary>
        /// <param name="nodeType">The type of node added by the navigation providers.</param>
        /// <returns>A sequence of navigation providers.</returns>
        public IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType) {
            return GetProviders()
                .OrderBy(np => np.GetPathLength())
                .Where(np => np.NodeType == nodeType)
                .ToArray();
        }
    }
}