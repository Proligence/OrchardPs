namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The base class for VFS nodes which contain child items.
    /// </summary>
    public class ContainerNode : VfsNode 
    {
        /// <summary>
        /// The node's static child items.
        /// </summary>
        private readonly List<VfsNode> staticNodes;

        public ContainerNode(IPowerShellVfs vfs, string name) 
            : base(vfs, name) 
        {
            this.staticNodes = new List<VfsNode>();
            this.NewItemName = "New item";
        }

        public ContainerNode(IPowerShellVfs vfs, string name, IEnumerable<VfsNode> staticNodes) 
            : base(vfs, name) 
        {
            this.staticNodes = new List<VfsNode>();
            this.NewItemName = "New item";

            if (staticNodes != null)
            {
                foreach (VfsNode node in staticNodes)
                {
                    this.AddStaticNode(node);
                }
            }
        }

        /// <summary>
        /// Gets or sets the action which implements the <c>New-Item</c> cmdlet.
        /// </summary>
        public Action<VfsNode, string, object> NewItemHandler { get; protected set; }
        
        /// <summary>
        /// Gets or sets the name of the action performed by the <c>New-Item</c> cmdlet.
        /// </summary>
        public string NewItemName { get; protected set; }

        public bool HasChildItems 
        {
            get 
            {
                if (this.staticNodes.Count > 0) 
                {
                    return true;
                }

                IEnumerable<VfsNode> nodes = this.GetVirtualNodes();
                if ((nodes != null) && nodes.Any()) 
                {
                    return true;
                }

                return false;
            }
        }

        public IEnumerable<VfsNode> StaticNodes 
        {
            get { return this.staticNodes; }
        }

        public virtual IEnumerable<VfsNode> GetVirtualNodes() 
        {
            return new VfsNode[0];
        }

        public IEnumerable<VfsNode> GetChildNodes(bool recurse = false) 
        {
            var nodes = new List<VfsNode>(this.staticNodes);
            
            IEnumerable<VfsNode> virtualNodes = this.GetVirtualNodes();
            if (virtualNodes != null) 
            {
                nodes.AddRange(virtualNodes);
            }

            nodes.ForEach(node => node.Parent = this);
            nodes = nodes.OrderBy(node => node.Name).ToList();

            if (recurse) 
            {
                var subnodes = new List<VfsNode>();
                foreach (VfsNode node in nodes) 
                {
                    var containerNode = node as ContainerNode;
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
        public override VfsNode NavigatePath(string path) 
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            VfsNode node = base.NavigatePath(path);
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

                VfsNode nextNode = containerNode.StaticNodes.FirstOrDefault(
                    n => n.Name.Equals(pathParts[index], StringComparison.Ordinal));

                if (nextNode == null) 
                {
                    IEnumerable<VfsNode> virtualNodes = containerNode.GetVirtualNodes().ToArray();
                    if (virtualNodes.Any()) 
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
        internal void AddStaticNode(VfsNode node) 
        {
            this.staticNodes.Add(node);
            node.Parent = this;
        }
    }
}