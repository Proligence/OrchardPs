// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PsNavigationProvider.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using Proligence.PowerShell.Vfs.Core;

    /// <summary>
    /// The base class for classes which extend the PowerShell Virtual File System by adding new static nodes to the
    /// VFS tree.
    /// </summary>
    public abstract class PsNavigationProvider : IPsNavigationProvider 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PsNavigationProvider"/> class.
        /// </summary>
        protected PsNavigationProvider() 
        {
            this.Path = "\\";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsNavigationProvider"/> class.
        /// </summary>
        /// <param name="nodeType">The type of the added node.</param>
        /// <param name="node">The added node.</param>
        protected PsNavigationProvider(NodeType nodeType, VfsNode node) 
        {
            this.Path = "\\";
            this.NodeType = nodeType;
            this.Node = node;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PsNavigationProvider"/> class.
        /// </summary>
        /// <param name="nodeType">The type of the added node.</param>
        /// <param name="path">The path of the added node's parent node.</param>
        /// <param name="node">The added node.</param>
        protected PsNavigationProvider(NodeType nodeType, string path, VfsNode node) 
        {
            this.Path = path;
            this.NodeType = nodeType;
            this.Node = node;
        }

        /// <summary>
        /// Gets or sets the path of the added node's parent node.
        /// </summary>
        public string Path { get; protected set; }

        /// <summary>
        /// Gets or sets the type of the added node.
        /// </summary>
        public NodeType NodeType { get; protected set; }

        /// <summary>
        /// Gets or sets the added node.
        /// </summary>
        public VfsNode Node { get; protected set; }

        /// <summary>
        /// Gets or sets the PowerShell VFS instance.
        /// </summary>
        public IPowerShellVfs Vfs { get; set; }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public virtual void Initialize()
        {
        }
    }
}