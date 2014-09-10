﻿namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implements a node which works as a symbolic link (shortcut) to another node in the PowerShell VFS.
    /// </summary>
    public class SymbolicLinkNode : ContainerNode 
    {
        public SymbolicLinkNode(IPowerShellVfs vfs, string name, VfsNode node) 
            : base(vfs, name) 
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

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

                foreach (VfsNode staticNode in containerNode.StaticNodes) 
                {
                    this.AddStaticNode(staticNode);
                }
            }
        }

        /// <summary>
        /// Gets or sets the target node of the symbolic link.
        /// </summary>
        public VfsNode TargetNode { get; protected set; }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() 
        {
            var containerNode = this.TargetNode as ContainerNode;
            if (containerNode == null) 
            {
                return new VfsNode[0];
            }

            return containerNode.GetVirtualNodes();
        }
    }
}