// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeType.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    /// <summary>
    /// Defines valid types of nodes in the Orchard VFS.
    /// </summary>
    public enum NodeType 
    {
        /// <summary>
        /// Specifies that the node should be added as a subnode of the root (drive) node.
        /// </summary>
        Global,

        /// <summary>
        /// Specifies that the node should be added as a subnode of each site node.
        /// </summary>
        Site,

        /// <summary>
        /// Specifies that the node is a custom node which should be handled by one of the custom PS navigation
        /// providers.
        /// </summary>
        Custom
    }
}