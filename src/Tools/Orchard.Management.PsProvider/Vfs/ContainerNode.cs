// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContainerNode.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The base class for VFS nodes which contain child items.
    /// </summary>
    public class ContainerNode : OrchardVfsNode 
    {
        /// <summary>
        /// The node's static child items.
        /// </summary>
        private readonly List<OrchardVfsNode> staticNodes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNode"/> class.
        /// </summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        public ContainerNode(IOrchardVfs vfs, string name) 
            : base(vfs, name) 
        {
            this.staticNodes = new List<OrchardVfsNode>();
            this.NewItemName = "New item";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerNode"/> class.
        /// </summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="staticNodes">The node's static child items.</param>
        public ContainerNode(IOrchardVfs vfs, string name, IEnumerable<OrchardVfsNode> staticNodes) 
            : base(vfs, name) 
        {
            this.staticNodes = new List<OrchardVfsNode>();
            this.NewItemName = "New item";

            if (staticNodes != null)
            {
                foreach (OrchardVfsNode node in staticNodes)
                {
                    this.AddStaticNode(node);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action which implements the <c>New-Item</c> cmdlet.
        /// </summary>
        public Action<OrchardVfsNode, string, object> NewItemHandler { get; protected set; }
        
        /// <summary>
        /// Gets or sets the name of the action performed by the <c>New-Item</c> cmdlet.
        /// </summary>
        public string NewItemName { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the node has any child nodes.
        /// </summary>
        public bool HasChildItems 
        {
            get 
            {
                if (this.staticNodes.Count > 0) 
                {
                    return true;
                }

                IEnumerable<OrchardVfsNode> nodes = this.GetVirtualNodes();
                if ((nodes != null) && (nodes.Count() > 0)) 
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the node's static child nodes.
        /// </summary>
        public IEnumerable<OrchardVfsNode> StaticNodes 
        {
            get { return this.staticNodes; }
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public virtual IEnumerable<OrchardVfsNode> GetVirtualNodes() 
        {
            return new OrchardVfsNode[0];
        }

        /// <summary>
        /// Gets the node's child nodes.
        /// </summary>
        /// <param name="recurse">
        /// <c>true</c> to include all child nodes or <c>false</c> to include only direct (first level) child nodes.
        /// </param>
        /// <returns>A sequence of child nodes.</returns>
        public IEnumerable<OrchardVfsNode> GetChildNodes(bool recurse = false) 
        {
            var nodes = new List<OrchardVfsNode>(this.staticNodes);
            
            IEnumerable<OrchardVfsNode> virtualNodes = this.GetVirtualNodes();
            if (virtualNodes != null) 
            {
                nodes.AddRange(virtualNodes);
            }

            nodes.ForEach(node => node.Parent = this);
            nodes = nodes.OrderBy(node => node.Name).ToList();

            if (recurse) 
            {
                var subnodes = new List<OrchardVfsNode>();
                foreach (OrchardVfsNode node in nodes) 
                {
                    ContainerNode containerNode = node as ContainerNode;
                    if (containerNode != null) 
                    {
                        subnodes.AddRange(containerNode.GetChildNodes(true));
                    }
                }

                nodes.AddRange(subnodes);
            }

            return nodes;
        }

        /// <summary>
        /// Gets the child node under the specified relative path.
        /// </summary>
        /// <param name="path">The relative path of the child node to get.</param>
        /// <returns>
        /// The child node under the specified relative path or <c>null</c> if there is no child node under the
        /// specified relative path.
        /// </returns>
        public override OrchardVfsNode NavigatePath(string path) 
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            OrchardVfsNode node = base.NavigatePath(path);
            if (node != null) 
            {
                return node;
            }

            if (path.StartsWith("\\", StringComparison.Ordinal))
            {
                path = path.Substring(1);
            }

            string[] pathParts = path.Split(new[] { '\\' });

            node = this;
            for (int i = 0; i < pathParts.Length; i++) 
            {
                int index = i;

                var containerNode = node as ContainerNode;
                if (containerNode == null) 
                {
                    return null;
                }

                OrchardVfsNode nextNode = containerNode.StaticNodes.FirstOrDefault(
                    n => n.Name.Equals(pathParts[index], StringComparison.Ordinal));

                if (nextNode == null) 
                {
                    IEnumerable<OrchardVfsNode> virtualNodes = containerNode.GetVirtualNodes();
                    if (virtualNodes != null) 
                    {
                        nextNode = virtualNodes.FirstOrDefault(
                            n => n.Name.Equals(pathParts[index], StringComparison.Ordinal));
                    }
                }

                if (nextNode == null) 
                {
                    return null;
                }

                node = nextNode;
            }

            return node;
        }

        /// <summary>
        /// Adds the specified static node to the node's child nodes.
        /// </summary>
        /// <param name="node">The static node to add.</param>
        internal void AddStaticNode(OrchardVfsNode node) 
        {
            this.staticNodes.Add(node);
            node.Parent = this;
        }
    }
}