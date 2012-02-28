// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PsNavigationProvider.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using Orchard.Management.PsProvider.Agents;

    /// <summary>
    /// The base class for classes which extend the Orchard Virtual File System by adding new static nodes to the VFS
    /// tree.
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
        protected PsNavigationProvider(NodeType nodeType, OrchardVfsNode node) 
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
        protected PsNavigationProvider(NodeType nodeType, string path, OrchardVfsNode node) 
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
        public OrchardVfsNode Node { get; protected set; }

        /// <summary>
        /// Gets or sets the agent manager instance.
        /// </summary>
        public IAgentManager AgentManager { get; set; }
        
        /// <summary>
        /// Gets or sets the Orchard VFS instance.
        /// </summary>
        public IOrchardVfs Vfs { get; set; }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public virtual void Initialize()
        {
        }
    }
}