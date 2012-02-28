// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPsNavigationProvider.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    /// <summary>
    /// Defines the interface for classes which extend the Orchard Virtual File System by adding new static nodes to
    /// the VFS tree.
    /// </summary>
    public interface IPsNavigationProvider 
    {
        /// <summary>
        /// Gets the path of the added node's parent node.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the type of the added node.
        /// </summary>
        NodeType NodeType { get; }

        /// <summary>
        /// Gets the added node.
        /// </summary>
        OrchardVfsNode Node { get; }
        
        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        void Initialize();
    }
}