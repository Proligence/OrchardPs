// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SymbolicLinkNode.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    using System.Collections.Generic;

    /// <summary>
    /// Implements a node which works as a symbolic link (shortcut) to another node in the Orchard VFS.
    /// </summary>
    public class SymbolicLinkNode : ContainerNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymbolicLinkNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which this node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="node">The target node of the symbolic link.</param>
        public SymbolicLinkNode(IOrchardVfs vfs, string name, OrchardVfsNode node) 
            : base(vfs, name) 
        {
            this.TargetNode = node;
            this.Item = node.Item;
            
            this.InvokeDefaultActionHandler = node.InvokeDefaultActionHandler;
            this.InvokeDefaultActionName = node.InvokeDefaultActionName;
            this.SetItemHandler = node.SetItemHandler;
            this.SetItemName = node.SetItemName;
            this.ClearItemHandler = node.ClearItemHandler;
            this.ClearItemName = node.ClearItemName;

            var containerNode = node as ContainerNode;
            if (containerNode != null) 
            {
                this.NewItemHandler = containerNode.NewItemHandler;
                this.NewItemName = containerNode.NewItemName;
                this.CopyItemHandler = containerNode.CopyItemHandler;
                this.CopyItemName = containerNode.CopyItemName;
                this.MoveItemHandler = containerNode.MoveItemHandler;
                this.MoveItemName = containerNode.MoveItemName;
                this.RemoveItemHandler = containerNode.RemoveItemHandler;
                this.RemoveItemName = containerNode.RemoveItemName;
                this.RenameItemHandler = containerNode.RenameItemHandler;
                this.RenameItemName = containerNode.RemoveItemName;

                foreach (OrchardVfsNode staticNode in containerNode.StaticNodes) 
                {
                    this.AddStaticNode(staticNode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the target node of the symbolic link.
        /// </summary>
        public OrchardVfsNode TargetNode { get; protected set; }

        /// <summary>
        /// Gets the node's virtual child nodes.
        /// </summary>
        /// <returns>Sequence of node.</returns>
        public override IEnumerable<OrchardVfsNode> GetVirtualNodes() 
        {
            var containerNode = this.TargetNode as ContainerNode;
            if (containerNode == null) 
            {
                return new OrchardVfsNode[0];
            }

            return containerNode.GetVirtualNodes();
        }
    }
}