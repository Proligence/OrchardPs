namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implements the PS navigation provider manager.
    /// </summary>
    public class NavigationProviderManager : INavigationProviderManager 
    {
        private readonly IPsNavigationProvider[] navigationProviders;

        public NavigationProviderManager(IEnumerable<IPsNavigationProvider> navigationProviders)
        {
            this.navigationProviders = navigationProviders.ToArray();
        }

        /// <summary>
        /// Gets all registered PS navigation providers.
        /// </summary>
        /// <returns>A sequence of navigation providers.</returns>
        public IEnumerable<IPsNavigationProvider> GetProviders()
        {
            return (IEnumerable<IPsNavigationProvider>)this.navigationProviders.Clone();
        }

        /// <summary>
        /// Gets all registered PS navigation providers which add a node of the specified type.
        /// </summary>
        /// <param name="nodeType">The type of node added by the navigation providers.</param>
        /// <returns>A sequence of navigation providers.</returns>
        public IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType) 
        {
            return this.GetProviders()
                .OrderBy(np => np.GetPathLength())
                .Where(np => np.NodeType == nodeType)
                .ToArray();
        }
    }
}