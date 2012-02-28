// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOrchardVfs.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    /// <summary>
    /// Defines the interface for objects which implement the Orchard Virtual File System (VFS).
    /// </summary>
    public interface IOrchardVfs 
    {
        /// <summary>
        /// Gets the root node of the VFS.
        /// </summary>
        RootVfsNode Root { get; }

        /// <summary>
        /// Gets the navigation provider manager for the VFS instance.
        /// </summary>
        INavigationProviderManager NavigationProviderManager { get; }
        
        /// <summary>
        /// Initializes the VFS instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Gets the node under the specified path in the VFS.
        /// </summary>
        /// <param name="path">The path of the node to get.</param>
        /// <returns>
        /// The node under the specified path or <c>null</c> if there is no node under the specified path.
        /// </returns>
        OrchardVfsNode NavigatePath(string path);
    }
}