namespace Orchard.Management.PsProvider.Vfs.Navigation
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the interface for classes which implement the PS navigation provider manager.
    /// </summary>
    public interface INavigationProviderManager 
    {
        /// <summary>
        /// Gets all registered PS navigation providers.
        /// </summary>
        /// <returns>A sequence of navigation providers.</returns>
        IEnumerable<IPsNavigationProvider> GetProviders();

        /// <summary>
        /// Gets all registered PS navigation providers which add a node of the specified type.
        /// </summary>
        /// <param name="nodeType">The type of node added by the navigation providers.</param>
        /// <returns>A sequence of navigation providers.</returns>
        IEnumerable<IPsNavigationProvider> GetProviders(NodeType nodeType);
    }
}