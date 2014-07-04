// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeType.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    /// <summary>
    /// Defines valid types of nodes in the PowerShell VFS tree.
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